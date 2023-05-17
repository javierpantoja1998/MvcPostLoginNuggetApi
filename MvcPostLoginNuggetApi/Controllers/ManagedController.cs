using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MvcPostLoginNuggetApi.Services;
using System.Security.Claims;

namespace MvcPostLoginNuggetApi.Controllers
{
    public class ManagedController : Controller
    {
        private ServiceApiUsuarios service;

        public ManagedController(ServiceApiUsuarios service)
        {
            this.service = service;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login
     (string email, string password)
        {
            string token =
                await this.service.GetTokenAsync(email, password);
            if (token == null)
            {
                ViewData["MENSAJE"] = "Email/Password incorrectos";
                return View();
            }
            else
            {
                ViewData["MENSAJE"] = "Bienvenid@ a mi App!!!";
                HttpContext.Session.SetString("TOKEN", token);
                ClaimsIdentity identity =
                    new ClaimsIdentity
                    (CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.Name, email));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, password));
                ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync
                    (CookieAuthenticationDefaults.AuthenticationScheme
                    , userPrincipal, new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });
                return RedirectToAction("Index", "Managed");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync
                (CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            return RedirectToAction("Index", "Managed");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ErrorAcceso()
        {
            return View();
        }
    }
}
