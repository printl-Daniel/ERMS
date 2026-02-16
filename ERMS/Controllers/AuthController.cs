using ERMS.DTOs.Auth;
using ERMS.Helpers;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ERMS.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If already logged in, redirect to home
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loginDto = model.ViewModelToDto();
            var result = await _authService.LoginAsync(loginDto);

            if (result.Success)
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new Claim(ClaimTypes.Name, result.FullName),
                    new Claim(ClaimTypes.Role, result.Role),
                    new Claim("EmployeeId", result.EmployeeId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Keep session for backward compatibility (optional - can be removed later)
                HttpContext.Session.SetString("UserId", result.UserId.ToString());
                HttpContext.Session.SetString("EmployeeId", result.EmployeeId.ToString());
                HttpContext.Session.SetString("FullName", result.FullName);
                HttpContext.Session.SetString("Role", result.Role);

                // Redirect based on role
                return result.Role switch
                {
                    "Admin" => RedirectToAction("Index", "Employee"),
                    "Manager" => RedirectToAction("Dashboard", "Manager"),
                    _ => RedirectToAction("Profile", "Employee")
                };
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}