using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Wyświetlamy widok z formularzem logowania 
            // lub coś prostego, np. hasło + login na sztywno
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            // SPRAWDZAMY login i hasło (np. "admin"/"test" na sztywno)
            if (username == "admin" && password == "test")
            {
                // Jeśli poprawne dane -> logujemy 
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);

                // Jeśli jest jakiś ReturnUrl, to wracamy tam
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                
                return RedirectToAction("Index", "Home");
            }

            // Jeśli dane niepoprawne -> ponowny widok
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.ErrorMessage = "Invalid login or password.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }
    }
}