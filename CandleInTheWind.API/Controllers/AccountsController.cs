using CandleInTheWind.API.Models.Accounts;
using CandleInTheWind.Data;
using CandleInTheWind.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CandleInTheWind.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AccountsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/Accounts/SignUp
        [HttpPost]
        [Route("SignUp")]
        public async Task<ActionResult> SignUp([FromBody]SignUpDTO dto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(new AccountResponseDTO()
                {
                    Success = true,
                    ErrorMessage = "Bạn đã đăng nhập"
                });
            }

            // check xem các trường thông tin có hợp lệ ko
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("error", "Đăng ký không thành công");
                return BadRequest(ModelState);
            }

            // check xem email đã được dùng để đăng kí trước đó hay chưa
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user != null)
                return Conflict(new AccountResponseDTO()
                {
                    Success = false,
                    ErrorMessage = "Email này đã được đăng ký. Vui lòng sử dụng email khác"
                });

            // tạo user mới
            var newUser = new User()
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
            };
            var passwordHasher = new PasswordHasher<User>();
            newUser.PasswordHash = passwordHasher.HashPassword(user, dto.Password);
            await _context.Users.AddAsync(newUser);
            var result = await _context.SaveChangesAsync();

            if (result == 0)
                return StatusCode(StatusCodes.Status500InternalServerError, new AccountResponseDTO()
                {
                    Success = false,
                    ErrorMessage = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại"
                });

            var token = GenerateToken(newUser);

            return Ok(new AccountResponseDTO()
            {
                Success = true,
                AccessToken = token,
            });
        }

        // POST: api/Accounts/SignIn
        [HttpPost]
        [Route("SignIn")]
        public async Task<ActionResult> SignIn([FromBody]SignInDTO dto)
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest(new AccountResponseDTO()
                {
                    Success = true,
                    ErrorMessage = "Bạn đã đăng nhập"
                });
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("error", "Đăng nhập thất bại");
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                ModelState.AddModelError("error", "Email hoặc mật khẩu chưa chính xác");
                return BadRequest(ModelState);
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("error", "Email hoặc mật khẩu chưa chính xác");
                return BadRequest(ModelState);
            }

            var token = GenerateToken(user);
            return Ok(new AccountResponseDTO()
            {
                Success = true,
                AccessToken = token,
            });
        }

        private string GenerateToken(User user)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (user.Email == "admin@gmail.com")
                authClaims.Add(new Claim(ClaimTypes.Role, "Admin"));
            else
                authClaims.Add(new Claim(ClaimTypes.Role, "User"));

            var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Issuer = _config["Jwt:ValidIssuer"],
                Audience = _config["Jwt:ValidAudience"],
                Expires = DateTime.UtcNow.AddHours(12),
                Subject = new ClaimsIdentity(authClaims),
                SigningCredentials = new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256Signature),
            });

            return tokenHandler.WriteToken(token);
        }
    }
}
