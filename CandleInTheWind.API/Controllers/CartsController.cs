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
using CandleInTheWind.API.Extensions;

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
        public async Task<ActionResult> GetProductInCart()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
            {
                return BadRequest(new { Error = "Không xác định được người dùng" });
            }
            var id = int.Parse(userIdClaim.Value);

            var carts = _context.Carts.Where(cart => cart.UserId == id)
                                      .Include(cart => cart.Product);

            var removeCarts = carts.Where(cart => cart.Product.Stock == 0);
            if (removeCarts.Count() > 0)
            {
                _context.Carts.RemoveRange(removeCarts);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new
                    {
                        Error = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại"
                    });
            }

            var cartResponse = (await carts.Where(cart => cart.Product.Stock > 0).ToListAsync()).Select(cart => cart.ToDTO());
            return Ok(new CartListDTO()
            {
                Products = cartResponse,
                ProductCount = cartResponse.Count(),
                TotalPrice = cartResponse.Sum(cart => cart.Price),
            });
            
        }

        // PUT: api/Carts/ChangeQuantity?productId=1&quantity=2
        [HttpPut("ChangeQuantity")]
        public async Task<ActionResult> ChangeQuantity([FromQuery]int productId, [FromQuery]int quantity)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
            {
                return BadRequest(new { Error = "Không xác định được người dùng" });
            }
            var userId = int.Parse(userIdClaim.Value);

            if (productId <= 0)
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

        // POST: api/Carts?productId=1&quantity=1
        [HttpPost]
        public async Task<ActionResult> AddProductToCart([FromQuery]int productId, [FromQuery]int quantity = 1)
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

            if (productId <= 0)
                return BadRequest(new { Error = "Dữ liệu không hợp lệ" });
            if (quantity <= 0)
                return BadRequest(new { Error = "Số lượng sản phẩm phải lớn hơn 0" });

            var product = await _context.Products.FirstOrDefaultAsync(product => product.Id == productId);
            if (product == null)
                return NotFound(new { Error = "Không tìm thấy sản phẩm cần thêm" });

            if (product.Stock == 0)
                return BadRequest(new { Error = "Sản phẩm hiện đã hết hàng" });

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

            return Ok(new { Message = "Đã thêm vào giỏ hàng"});
        }

        // DELETE: api/Carts/5
        [HttpDelete("{productId}")]
        public async Task<ActionResult> DeleteProductInCart([FromRoute]int productId)
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
            {
                return BadRequest(new { Error = "Không xác định được người dùng" });
            }
            var userId = int.Parse(userIdClaim.Value);
            
            var cart = await _context.Carts.FindAsync(userId, productId);
            if (cart == null)
            {
                return NotFound(new { Error = "Không tìm thấy sản phẩm cần xóa" });
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Carts/CheckVoucher/1
        [HttpGet("CheckVoucher/{voucherId}")]
        public async Task<ActionResult> CheckVoucher(int? voucherId)
        {
            if (voucherId == null)
                return BadRequest(new { Error = "Chưa chọn mã giảm giá" });

            var voucher = await _context.Vouchers.FindAsync((int)voucherId);
            if (voucher == null)
                return NotFound(new { Error = "Không tìm thấy mã giảm giá" });

            var userClaimId = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userClaimId == null)
                return BadRequest(new { Error = "Không xác định được người dùng" });
            var userId = int.Parse(userClaimId.Value);

            var user = await _context.Users.FindAsync(userId);
            if (user.Points < voucher.Points)
                return Ok(new { success = false, totalPrice = -1 });

            var total = _context.Carts.Include(cart => cart.Product)
                                      .Where(cart => cart.UserId == userId)
                                      .Sum(cart => cart.Quantity * cart.Product.Price);
            return Ok(new { success = true, totalPrice = total * (decimal)(100 - voucher.Value) / 100});
        }

        private bool CartExists(int userId, int productId)
        {
            return _context.Carts.Any(cart => cart.UserId == userId && cart.ProductId == productId);
        }

        //private CartDTO ToCartDTO(Cart cart)
        //{
        //    var product = cart.Product;
        //    return new CartDTO()
        //    {
        //        ProductId = cart.ProductId,
        //        ProductName = product.Name,
        //        ProductImageUrl = product.ImageUrl,
        //        UnitPrice = product.Price,
        //        Stock = product.Stock,
        //        Quantity = cart.Quantity,
        //        Price = product.Price * cart.Quantity,
        //    };
        //}
    }
}
