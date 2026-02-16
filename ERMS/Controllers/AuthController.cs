using ERMS.DTOs.Auth;
using ERMS.Helpers;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Auth;
using Microsoft.AspNetCore.Mvc;

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
            if (HttpContext.Session.GetString("UserId") != null)
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
                // Set session variables
                HttpContext.Session.SetString("UserId", result.UserId.ToString());
                HttpContext.Session.SetString("EmployeeId", result.EmployeeId.ToString());
                HttpContext.Session.SetString("FullName", result.FullName);
                HttpContext.Session.SetString("Role", result.Role);

                // Redirect based on role
                if (result.Role == "Admin")
                {
                    return RedirectToAction("Index", "Employee");
                }
                else if (result.Role == "Manager")
                {
                    return RedirectToAction("Dashboard", "Manager");
                }
                else
                {
                    return RedirectToAction("Profile", "Employee");
                }
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }

}
