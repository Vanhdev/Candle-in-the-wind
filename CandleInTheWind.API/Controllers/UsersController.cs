using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using CandleInTheWind.API.Models.Users;
using Microsoft.AspNetCore.Identity;
using CandleInTheWind.API.Extensions;
using System;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: api/Users/Profile
        [HttpGet("Profile")]
        public async Task<ActionResult> GetProfile()
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

            return Ok(user.ToDTO());
        }

        // PUT: api/Users/UpdateInfo
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody]UserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Dữ liệu không hợp lệ");
                return BadRequest(ModelState);
            }

            // check xem ngày sinh có hợp lệ không
            if (dto.DateOfBirth != null)
            {
                if (dto.DateOfBirth >= DateTime.Now)
                    return BadRequest(new { Error = "Ngày sinh của bạn không được bằng hoặc sau ngày hiện tại" });
            }

            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest(new { Error = "Không xác định được người dùng" });
            
            var id = int.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { Error = "Không tìm thấy người dùng" });

            
            user.UserName = dto.UserName;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = dto.Gender;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại" });
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }


        [HttpPut("ChangePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody]UpdatePasswordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Error", "Dữ liệu không hợp lệ");
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sid);
            if (userIdClaim == null)
                return BadRequest(new { Error = "Không xác định được người dùng" });

            var id = int.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { Error = "Không tìm thấy người dùng" });

            var passwordHasher = new PasswordHasher<User>();
            var verifyPasswordResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);
            if (verifyPasswordResult == PasswordVerificationResult.Failed)
                return BadRequest(new { Error = "Mật khẩu không chính xác" });

            user.PasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                    return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại" });
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
    }
}
