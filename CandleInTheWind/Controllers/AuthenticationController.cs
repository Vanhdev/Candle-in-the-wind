using CandleInTheWind.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Distributed;

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
        public IActionResult Login([Bind("Email", "Password")] AdminAccount account)
        {
            if (account.Email.Equals(EMAIL) && account.Password.Equals(PASSWORD))
            {
                //_cache.SetString("login_admin", "admin");
                //HttpContext.Session.SetString("login_admin", "admin");
                return RedirectToActionPermanent("Index", "Home");
            }

            return RedirectToAction("Login", new { message = "Fail" });
        }
        public IActionResult Logout()
        {
            //_cache.Remove("login_admin");
            //HttpContext.Session.Remove("login_admin");
            return RedirectToActionPermanent("Login");
        }
    }
}