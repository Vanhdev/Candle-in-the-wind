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
using CandleInTheWind.API.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        //{
        //    return await _context.Users.ToListAsync();
        //}

        // GET: api/Users/Profile
        [HttpGet("Profile")]
        [Authorize]
        public async Task<ActionResult<ProfileDTO>> GetProfile()
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

            var userRepsonse = ToProfileDTO(user);
            return Ok(userRepsonse);
        }

        // PUT: api/Users/UpdateInfo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody]UserDTO dto)
        {
            //if (id != user.Id)
            //{
            //    return BadRequest();
            //}
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

            
            user.UserName = dto.UserName;
            user.PhoneNumber = dto.PhoneNumber;
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


        [HttpPut("UpdatePassword")]
        [Authorize]
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<User>> PostUser(User user)
        //{
        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUser", new { id = user.Id }, user);
        //}

        // DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser(int id)
        //{
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool UserExists(int id)
        //{
        //    return _context.Users.Any(e => e.Id == id);
        //}

        private ProfileDTO ToProfileDTO(User user)
        {
            return new ProfileDTO()
            {
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Points = user.Points,
            };
        }
    }
}
