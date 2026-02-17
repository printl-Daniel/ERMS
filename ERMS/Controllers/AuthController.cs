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

        // ── LOGIN GET ────────────────────────────────────────────────────────────
        // Facebook behaviour:
        //   • Already logged in  → skip the login page entirely, go straight to home
        //   • Not logged in      → wipe any stale / partial cookie, show the form
        [HttpGet]
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            // 1. Already authenticated → bounce straight to their dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }

            // 2. Not authenticated → clear any stale cookie + session before
            //    showing the form so the next login starts completely clean
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // ── LOGIN POST ───────────────────────────────────────────────────────────
        // Facebook behaviour:
        //   • Bad credentials      → stay on login, show error, keep form values
        //   • Good credentials     → sign in, write session, redirect to dashboard
        //   • "Remember Me" ticked → cookie survives browser close (persistent)
        //   • "Remember Me" off    → session cookie only, gone when browser closes
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var loginDto = model.ViewModelToDto();
            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }

            // ── Build claims identity ────────────────────────────────────────────
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Name,           result.FullName),
                new Claim(ClaimTypes.Role,           result.Role),
                new Claim("EmployeeId",              result.EmployeeId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // ── Cookie lifetime (Facebook-style) ────────────────────────────────
            //   RememberMe = true  → persistent 30-day cookie  (stays after browser close)
            //   RememberMe = false → session cookie            (deleted on browser close)
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                                    ? DateTimeOffset.UtcNow.AddDays(30)   // long-lived
                                    : DateTimeOffset.UtcNow.AddMinutes(30), // session-length
                AllowRefresh = true  // sliding expiry resets on each request
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // ── Mirror key values into session (kept for backward compat) ────────
            HttpContext.Session.SetString("UserId", result.UserId.ToString());
            HttpContext.Session.SetString("EmployeeId", result.EmployeeId.ToString());
            HttpContext.Session.SetString("FullName", result.FullName);
            HttpContext.Session.SetString("Role", result.Role);

            // ── Redirect ─────────────────────────────────────────────────────────
            // Honor returnUrl (e.g. user tried to open /Employee/Index while
            // logged out — send them there after login, like Facebook does).
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDashboard(result.Role);
        }

        // ── LOGOUT POST ──────────────────────────────────────────────────────────
        // Facebook behaviour: POST-only, wipes everything, lands on login page.
        // The login page's GET will then clear any remnants (double safety).
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

        // ── FORGOT PASSWORD ──────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var forgotPasswordDto = new ForgotPasswordDto { Email = model.Email };
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        // ── RESET PASSWORD ───────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Invalid reset link.";
                return RedirectToAction("Login");
            }

            var decodedToken = Uri.UnescapeDataString(token);
            var isValid = await _authService.ValidateResetTokenAsync(decodedToken);

            if (!isValid)
            {
                TempData["ErrorMessage"] = "This reset link is invalid or has expired.";
                return RedirectToAction("Login");
            }

            return View(new ResetPasswordViewModel { Token = decodedToken });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new ResetPasswordDto
            {
                Token = model.Token,
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            };

            var result = await _authService.ResetPasswordAsync(dto);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("ResetPasswordConfirmation");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();

        // ── ACCESS DENIED ────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ── HELPERS ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Redirect to the correct dashboard based on the current user's role claim.
        /// Pass a role string directly when the claim isn't written yet (post-login).
        /// </summary>
        private IActionResult RedirectToDashboard(string? role = null)
        {
            role ??= User.FindFirst(ClaimTypes.Role)?.Value;

            return role switch
            {
                "Admin" => RedirectToAction("Index", "Employee"),
                "Manager" => RedirectToAction("Dashboard", "Manager"),
                _ => RedirectToAction("Profile", "Employee")
            };
        }
    }
}