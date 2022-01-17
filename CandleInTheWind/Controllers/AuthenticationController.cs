using CandleInTheWind.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace CandleInTheWind.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly string EMAIL = "admin@gmail.com";
        private readonly string PASSWORD = "admin69";
        public AuthenticationController()
        {
        }

        [HttpGet]
        public IActionResult Login(string message = null)
        {
            
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email", "Password")] AdminAccount account)
        {
            if (account.Email.Equals(EMAIL) && account.Password.Equals(PASSWORD))
            {
                //_cache.SetString("login_admin", "admin");
                //HttpContext.Session.SetString("login_admin", "admin");
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@gmail.com")
                };
                var claimIdentity = new ClaimsIdentity(claims, "Admin Identity");
                var userPrincipal = new ClaimsPrincipal(new[] { claimIdentity });
                await HttpContext.SignInAsync(userPrincipal);
                return RedirectToActionPermanent("Index", "Home");
            }

            return RedirectToAction("Login", new { message = "Fail" });
        }
        public async Task<IActionResult> Logout()
        {
            //_cache.Remove("login_admin");
            //HttpContext.Session.Remove("login_admin");
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToActionPermanent("Login");
        }
    }
}