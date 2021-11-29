using CandleInTheWind.Data;
using CandleInTheWind.Models;
using CandleInTheWind.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace CandleInTheWind.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                //if (User.IsInRole("Admin"))
                //    return Json(new { Name = User.Identity.Name, Role = "Admin" });
                //else
                    return RedirectToAction("Index", "Home");
            return View(new LoginViewModel());
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                //if (User.IsInRole("Admin"))
                //    return Json(new { Name = User.Identity.Name, Role = "Admin" });
                //else
                    return RedirectToAction("Index", "Home");
            return View(new LoginViewModel());
        }

        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            returnUrl ??= Url.Content("~/");
            if (User.Identity.IsAuthenticated)
                return RedirectToLocal(returnUrl);
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.LoginEmail);
                if (user != null)
                {
                    var passwordHash = (new PasswordHasher<User>()).VerifyHashedPassword(user, user.PasswordHash, model.LoginPassword);
                    if (passwordHash == PasswordVerificationResult.Success)
                    {
                        var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.Name, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email)
                        };
                        var identity = new ClaimsIdentity(claims, "CookieAuth");
                        var claimPrincipal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync("CookieAuth", claimPrincipal);

                        return RedirectToLocal(returnUrl);
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác");
            ViewData["Error"] = "Login";
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
                //if (User.IsInRole("Admin"))
                //    return Json(new { Name = User.Identity.Name, Role = "Admin" });
                //else
                    return RedirectToAction("Index", "Home");
            //return View("/Views/Template/Register.cshtml", new RegisterViewModel());
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            returnUrl ??= Url.Content("~/");
            if (User.Identity.IsAuthenticated)
                return RedirectToLocal(returnUrl);
            if (ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == model.RegisterEmail);
                if (userInDb != null)
                {
                    ModelState.AddModelError("", "Email này đã được đăng ký");
                    ViewData["Error"] = "Register";
                    return View(model);
                }
                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.RegisterEmail,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                };
                user.PasswordHash = (new PasswordHasher<User>()).HashPassword(user, model.RegisterPassword);
                await _context.Users.AddAsync(user);
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName), 
                    new Claim(ClaimTypes.Email, user.Email)
                };
                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var claimPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", claimPrincipal);

                await _context.SaveChangesAsync();

                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ViewData["Error"] = "Register";
            return View(model);
        }

        public async Task<IActionResult> Logout(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            returnUrl ??= Url.Content("~/");
            //await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToLocal(returnUrl);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
