using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
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

      
  
        //GET: api/Order/MyOders
        [HttpGet("MyOrders")]
        [Authorize]
        public async Task<ActionResult> GetOrderByUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null) 
                return BadRequest();
            var userId = int.Parse(userIdClaim.Value);

            var orders = await _context.Orders.Include(order => order.User)
                                              .Include(order => order.Product)
                                              .Where(order => order.User.Id == userId)
                                              .ToListAsync();
            var responseOrders = orders.Select(order => toSimpleDTO(order));
            return Ok(responseOrders);

        }

        // GET: api/Orders/MyOrder/5
        [HttpGet("MyOrders/{OrderId}")]
        [Authorize]
        public async Task<ActionResult> GetOrder(int OrderId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();

            var userId = int.Parse(userIdClaim.Value);

            var order = await _context.Orders.Include(order => order.Product)
                                             .Include(order => order.Voucher)
                                             .Include(order => order.User)
                                             .Where(order => order.User.Id == userId)
                                             .FirstOrDefaultAsync(order => order.Id == OrderId);

            if (order == null)
            {
                return NotFound();
            }

            var responseOrder = toDTO(order);

            return Ok(responseOrder);
        }


        private SimpleOrderDTO toSimpleDTO(Order order)
        {
            return new SimpleOrderDTO
            {
                Id = order.Id,
                PurchaseDate = order.PurchasedDate,
                Status = order.Status,
                Total = order.Total,
                ProductName = order.Product.Name
            };
        }


        private OrderDTO toDTO(Order order)
        {
            return new OrderDTO
            {
                Id = order.Id,
                PurchaseDate = order.PurchasedDate,
                Status = order.Status,
                UnitPrice = order.UnitPrice,
                Quantity = order.Quantity,
                Total = order.Total,
                ProductId = order.Product.Id,
                ProductName = order.Product.Name,
                VoucherId = order.Voucher?.Id,
                VoucherName = order.Voucher?.Name,
                UserId = order.User.Id,

            };
        }



        
        // PUT: api/Orders/MyOrders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("MyOrders/Cancel/{OrderId}")]
        [Authorize]
        public async Task<IActionResult> PutOrder(int OrderId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest();
            
            var userId = int.Parse(userIdClaim.Value);

            var order = await _context.Orders.Include(order => order.User)
                                             .Where(order => order.User.Id == userId)
                                             .FirstOrDefaultAsync(order => order.Id == OrderId);

            if (order == null)
                return NotFound(new {Error = "Không tìm thấy đơn hàng"});
            
            if(order.Status.Equals(OrderStatus.Canceled))
            {
                return BadRequest(new { Error = "Đơn hàng đã bị huỷ từ trước" });
            }
            else
            {
                order.Status = OrderStatus.Canceled;
            }

            _context.Entry(order).State = EntityState.Modified;

            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(OrderId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            } 

            return Ok("Huỷ đơn hàng thành công.");
        }

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
        */
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
        
        

    }
}
