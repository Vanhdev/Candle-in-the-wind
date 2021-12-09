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
using CandleInTheWind.API.Models.Carts;
using System.IdentityModel.Tokens.Jwt;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<CartListDTO>> GetProductInCart()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
            {
                return BadRequest(new { Error = "Không xác định được người dùng" });
            }
            var id = int.Parse(userIdClaim.Value);

            if (!_context.Users.Any(user => user.Id == id))
            {
                return NotFound();
            }

            var carts = await _context.Carts.Where(cart => cart.UserId == id).Include(cart => cart.Product).ToListAsync();
            var cartResponse = carts.Select(cart => ToCartDTO(cart));
            return Ok(new CartListDTO()
            {
                Products = cartResponse,
                ProductCount = cartResponse.Count(),
                TotalPrice = cartResponse.Sum(cart => cart.Price),
            });
            
        }

        // GET: api/Carts/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Cart>> GetCart(int id)
        //{
        //    var cart = await _context.Carts.FindAsync(id);

        //    if (cart == null)
        //    {
        //        return NotFound();
        //    }

        //    return cart;
        //}

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPut()]
        public async Task<ActionResult<UpdateQuantityDTO>> UpdateQuantity([FromBody]int productId, [FromBody]int quantity)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
            {
                return BadRequest(new { Error = "Không xác định được người dùng" });
            }
            var userId = int.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (productId < 0)
                return BadRequest(new { Error = "Dữ liệu không hợp lệ" });
            if (quantity <= 0)
                return BadRequest(new { Error = "Số lượng sản phẩm phải lớn hơn 0" });

            var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == productId);
            if (product == null)
                return NotFound(new { Error = "Không tìm thấy sản phẩm" });

            var cart = await _context.Carts.FindAsync(userId, productId);
            if (cart == null)
                return NotFound(new { Error = "Không tìm thấy sản phẩm" });
            if (quantity > product.Stock)
                quantity = product.Stock;
            cart.Quantity = quantity;
            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại" });
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok(new UpdateQuantityDTO()
            {
                ProductId = productId,
                UnitPrice = product.Price,
                Quantity = quantity,
                Price = product.Price * quantity
            });
        }

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddProductToCart([FromBody]int productId, [FromBody]int quantity = 1)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
            {
                return BadRequest(new { Error = "Không xác định được người dùng" });
            }
            var id = int.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (productId < 0)
                return BadRequest(new { Error = "Dữ liệu không hợp lệ" });
            if (quantity <= 0)
                return BadRequest(new { Error = "Số lượng sản phẩm phải lớn hơn 0" });

            var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == productId);
            if (product == null)
                return NotFound(new { Error = "Không tìm thấy sản phẩm cần thêm" });

            if (CartExists(id, productId))
                return Conflict(new { Error = "Giỏ hàng của bạn đã có sản phẩm này" });

            var newCart = new Cart()
            {
                UserId = id,
                User = user,
                ProductId = productId,
                Product = product,
                Quantity = quantity
            };
            _context.Carts.Add(newCart);
            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Error = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại" });
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok();
        }

        // DELETE: api/Carts/5
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductInCart(int productId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
            {
                return BadRequest(new { Error = "Không xác định được người dùng" });
            }
            var userId = int.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound(new { Error = "Không tìm thấy sản phẩm cần xóa" });
            }

            var cart = await _context.Carts.FindAsync(userId, productId);
            if (cart == null)
            {
                return NotFound(new { Error = "Không tìm thấy sản phẩm cần xóa" });
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartExists(int userId, int productId)
        {
            return _context.Carts.Any(cart => cart.UserId == userId && cart.ProductId == productId);
        }

        private CartDTO ToCartDTO(Cart cart)
        {
            var product = cart.Product;
            return new CartDTO()
            {
                ProductId = cart.ProductId,
                ProductName = product.Name,
                ProductImageUrl = product.ImageUrl,
                UnitPrice = product.Price,
                Stock = product.Stock,
                Quantity = cart.Quantity,
                Price = product.Price * cart.Quantity,
            };
        }
    }
}
