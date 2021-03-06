using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using CandleInTheWind.API.Models.Orders;
using CandleInTheWind.API.Extensions;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders/MyOrders?pageIndex=1&pageSize=5
        [HttpGet("MyOrders")]
        [Authorize]
        public async Task<ActionResult> GetMyOrders([FromQuery]int pageIndex = 1, [FromQuery]int pageSize = 5)
        {
            if (pageSize <= 0) 
                pageSize = 5;
            if (pageIndex <= 0) 
                pageIndex = 1;

            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();
            
            var userId = int.Parse(userIdClaim.Value);

            var orders = _context.Orders.Include(order => order.User)
                                        .Include(order => order.OrderProducts) //doi tuong la Order
                                        .ThenInclude(op => op.Product) // doi tuong se la OrderProduct
                                        .Where(order => order.User.Id == userId);

            int count = orders.Count();
            int totalPages = count / pageSize + ((count % pageSize == 0) ? 0 : 1);
            if (pageIndex > totalPages)
                pageIndex = 1;

            var ordersPerPage = await orders.OrderByDescending(order => order.Id).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
            var responseOrders = ordersPerPage.Select(order => order.ToOrderDTO());

            return Ok(new OrderFilterDTO()
            {
                Orders = responseOrders,
                TotalPages = totalPages,
                PageSize = pageSize,
                PageIndex = pageIndex,
            });
        }

        // G??T: api/Orders/MyOrders/1
        [HttpGet("MyOrders/{orderId}"),Authorize]
        public async Task<ActionResult> GetDetailOrder([FromRoute]int orderId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var userId = int.Parse(userIdClaim.Value);

            var order = await _context.Orders.Include(order => order.User)
                                             .Include(order =>order.OrderProducts)
                                             .ThenInclude(op => op.Product)
                                             .Include(order => order.Voucher)
                                             .FirstOrDefaultAsync(order => order.Id == orderId && order.UserId == userId);

            if(order == null) 
                return NotFound(new {Error = "Kh??ng t??m th???y ????n h??ng" });

            var responseOrder = order.ToOrderDetailDTO();

            if (order.Voucher == null)
            {
                var orderProducts = await _context.OrderProducts.Include(op => op.Product)
                                                                .Where(op => op.OrderId == orderId)
                                                                .ToListAsync();

                decimal total = 0; // total price before deduction
                foreach (var p in orderProducts) // return all the product in the order
                    total += p.Product.Price * p.Quantity;

                responseOrder.Points = (int)(total - order.Total);
            }
            return Ok(responseOrder);
        }

        
        // POST: api/Orders
        [HttpPost("CreateOrder"), Authorize]
        public async Task<ActionResult> PostOrder(int? voucherId, int? points )
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users.FindAsync(userId);

            var total = await _context.Carts.Include(cart => cart.Product)
                                            .Where(cart => cart.UserId == userId)
                                            .SumAsync(cart => cart.Product.Price * cart.Quantity);


            var userPoint = user.Points; // get user's point from database
            if (points != null && points > 0 && voucherId != null) // cannot use points deduction and voucher at the same time
                return BadRequest(new { Error = "Ch??? ???????c ch???n m???t h??nh th???c gi???m gi??" });
            if (voucherId == null && points != null)
            {
                
                if (points > userPoint || points < 0)   // using point deduction
                    return BadRequest(new { Error = "??i???m kh??ng h???p l???" });
                else if (points > 0)
                {
                    if(points > total)   // minimum total is 0
                    {
                        user.Points -= (int)total;
                        total = 0;
                    }
                    else
                    {
                        total -= (int)points;
                        user.Points -= (int)points;
                    }
                }                    
            }
            
            if (voucherId != null && (points == null || points == 0)) // using voucher
            {
                var voucher = await _context.Vouchers.FindAsync(voucherId);
                if (voucher == null)
                    return NotFound(new {Error = "Kh??ng t??m th???y voucher" });
                if (userPoint < voucher.Points)
                    return BadRequest(new { Error = "Kh??ng ????? ??i???u ki???n ??p d???ng voucher" });
                if(voucher.Quantity == 0)
                    return BadRequest(new {Error = "Kh??ng c??n voucher" });

                total = total * (decimal)(100 - voucher.Value) / 100;
                voucher.Quantity--;  // decrease number of voucher in database
                user.Points -= voucher.Points; // decrease user point
            }

            var newOrder = new Order
            {
                PurchasedDate = DateTime.Now,
                Total = total,
                VoucherId = voucherId,
                UserId = userId
            };
            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            var carts = await _context.Carts.Include(cart => cart.Product).Where(cart => cart.UserId == userId).ToListAsync();

            foreach (var c in carts)
                c.Product.Stock -= c.Quantity; // decrease each product's stock in database
            

            if (carts.Count == 0) // no product in cart
                return BadRequest(new { Error = "Vui l??ng ch???n s???n ph???m tr?????c" });
            var newOrderProducts = carts.Select(cart => new OrderProduct
            {
                OrderId = newOrder.Id,
                ProductId = cart.ProductId,
                UnitPrice = cart.Product.Price,
                Quantity = cart.Quantity
            });

            _context.OrderProducts.AddRange(newOrderProducts);

             _context.Carts.RemoveRange(carts);  // remove product in cart
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetDetailOrder", new { OrderId = newOrder.Id });
            return Ok(new { Message = "????n h??ng ??ang ???????c x??? l??, c???m ??n qu?? kh??ch" });
        }

        // PUT: api/Orders/MyOrder/1
        [HttpPut("MyOrder/{orderId}"), Authorize]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var userId = int.Parse(userIdClaim.Value);

            var user = await _context.Users.FindAsync(userId);

            var order = await _context.Orders.Include(order => order.User)
                                             .Include(order => order.Voucher)
                                             .FirstOrDefaultAsync(order => order.Id == orderId && order.UserId == userId);

            if(order == null)
                return NotFound(new {Error = "Kh??ng t??m th???y ????n h??ng" });

            if (order.Status == OrderStatus.Approved || order.Status == OrderStatus.NotApproved || order.Status == OrderStatus.Canceled)
                return BadRequest(new { Error = "Kh??ng th??? hu??? ????n h??ng" });
            else
            {
                order.Status = OrderStatus.Canceled;
                if (order.VoucherId != null) // return the voucher if used
                {

                    order.Voucher.Quantity++;
                    user.Points += order.Voucher.Points; //return used point
                }

                var orderProducts = await _context.OrderProducts.Include(op => op.Product)
                                                                .Where(op => op.OrderId == orderId)
                                                                .ToListAsync();

                decimal total = 0; // total price before deduction
                foreach (var p in orderProducts) // return all the product in the order
                {
                    p.Product.Stock += p.Quantity;
                    total += p.Product.Price * p.Quantity;
                }
                if(order.VoucherId == null)
                    user.Points += (int)(total - order.Total); //return points for the users 

                await _context.SaveChangesAsync(); // Save changes (product, voucher?, points?, orderStatus)

                return Ok(new { Message = "Hu??? ????n h??ng th??nh c??ng" });
            }
        }


        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
