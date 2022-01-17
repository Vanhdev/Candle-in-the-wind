using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using Microsoft.AspNetCore.Authorization;

namespace CandleInTheWind.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.User).Include(o => o.Voucher);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Voucher)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,PurchasedDate,Status,Total,VoucherId,UserId")] Order order)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(order);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
        //    ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Name", order.VoucherId);
        //    return View(order);
        //}

        public ActionResult CreateViewModel(OrderViewModel viewModel)
        {
            var user = _context.Users.FirstOrDefault(a => a.Id == viewModel.UserID);
            var voucher = _context.Vouchers.FirstOrDefault(a => a.Id == viewModel.VoucherID);

            var order = new Order { UserId = viewModel.UserID, User = user, VoucherId = viewModel.VoucherID, Voucher = voucher };

            _context.Add(order);
            _context.SaveChanges();

            List<OrderProduct> orderProducts = new List<OrderProduct>();

            for (int i = 0; i < viewModel.ProductID.Count; i++)
            {
                var product = _context.Products.FirstOrDefault(a => a.Id == viewModel.ProductID[i]);
                orderProducts.Add(new OrderProduct { OrderId = order.Id, 
                                                    ProductId = viewModel.ProductID[i], 
                                                    Quantity = viewModel.Quantity[i], 
                                                    Product = product, 
                                                    Order = order,
                                                    UnitPrice = product.Price});
            }

            int total = 0;

            foreach(var orderProduct in orderProducts)
            {
                total += orderProduct.Quantity * (int)orderProduct.UnitPrice;

                _context.OrderProducts.Add(orderProduct);
            }

            order.User.Points += (int)(0.05 * total);

            order.Total = (order.Voucher == null) ? total : (int)(total - total * order.Voucher.Value / 100);

            if (order.Voucher != null && order.User.Points > order.Voucher.Points && order.Voucher.Quantity > 0)
            {
                order.User.Points -= order.Voucher.Points;
                order.Voucher.Quantity -= 1;
            }

            _context.AddRange(orderProducts);

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Edit/5
        public ActionResult Approve(int id, OrderStatus Status)
        {
            var order = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Voucher)
                .Include(o => o.OrderProducts)
                .FirstOrDefault(a => a.Id == id);

            int total = 0;
            foreach(var op in order.OrderProducts)
            {
                total += op.Quantity * (int)op.UnitPrice;
            }    

            order.Status = Status;

            if(Status == OrderStatus.Canceled || Status == OrderStatus.NotApproved)
            {
                if (order.Voucher != null)
                {
                    order.User.Points += order.Voucher.Points;

                    order.Voucher.Quantity += 1;

                    order.User.Points -= (int)(0.05 * (double)(order.Total));
                }
                else if(order.Total < total)
                {
                    int point = total - (int)order.Total;

                    order.User.Points += point;
                }    
                
            }    

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PurchasedDate,Status,Total,VoucherId,UserId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", order.UserId);
            ViewData["VoucherId"] = new SelectList(_context.Vouchers, "Id", "Name", order.VoucherId);
            return View(order);
        }

        // GET: Orders/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Orders
        //        .Include(o => o.User)
        //        .Include(o => o.Voucher)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}

        // POST: Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == id);
            _context.Orders.Remove(order);
            _context.SaveChanges();

            return Ok();
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
