using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using CandleInTheWind.API.Models.Orders;

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

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/MyOrders
        [HttpGet("MyOrders")]
        [Authorize]
        public async Task<ActionResult<Order>> GetMyOrders()
        {

            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();
            
            var userId = int.Parse(userIdClaim.Value);

            var orders = await _context.Orders.Include(order => order.User)
                                              .Include(order => order.OrderProducts) //doi tuong la Order
                                              .ThenInclude(op => op.Product) // doi tuong se la OrderProduct
                                              .Where(order => order.User.Id == userId)
                                              .ToListAsync();

            var responseOrders = orders.Select(order => toSimpleDTO(order));
            

            //var order = await _context.Orders.FindAsync();

            return Ok(responseOrders);
        }

        [HttpGet("MyOrders/{OrderId}"),Authorize]
        public async Task<ActionResult> GetDetailOrder([FromRoute]int OrderId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var userId = int.Parse(userIdClaim.Value);

            var order = await _context.Orders.Include(order => order.User)
                                             .Include(order =>order.OrderProducts)
                                             .ThenInclude(op => op.Product)
                                             .Include(order => order.Voucher)
                                             .FirstOrDefaultAsync(order => order.Id == OrderId && order.UserId == userId);

            if(order == null) 
                return NotFound(new {Error = "Không tìm thấy đơn hàng" });

            var responseOrder = toOrderDTO(order);
            return Ok(responseOrder );

        }




        /*
        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        */
        /*
        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
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

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
        */

        private SimpleOrderDTO toSimpleDTO(Order order)
        {
            return new SimpleOrderDTO
            {
                Id = order.Id,
                PurchaseDate = order.PurchasedDate,
                Total = order.Total,
                Status = order.Status,
                StatusName = order.Status.toStatusName(),
                ProductName = order.OrderProducts.Select(op => op.Product.Name)
            };
        }


        private OrderDTO toOrderDTO(Order order)
        {
            return new OrderDTO
            {
                UserId = order.UserId,
                UserName = order.User.UserName,
                Id = order.Id,
                PurchaseDate = order.PurchasedDate,
                Total = order.Total,
                Status = order.Status,
                StatusName = order.Status.toStatusName(),
                ProductName = order.OrderProducts.Select(op => op.Product.Name),
                ProductIDs = order.OrderProducts.Select(op => (int)op.ProductId),
                UnitPrices = order.OrderProducts.Select(op => op.Product.Price),
                Quantity = order.OrderProducts.Select(op => op.Quantity),
                ProductImageUrls = order.OrderProducts.Select(op => op.Product.ImageUrl),
                VoucherId = order.Voucher?.Id,
                VoucherName = order.Voucher?.Name,
                VoucherValue = order.Voucher?.Value
            };
        }





    }
}
