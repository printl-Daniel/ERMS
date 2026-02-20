using ERMS.DTOs.Auth;
using ERMS.Helpers;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Auth;
using ERMS.Constants;
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
        public async Task<IActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToDashboard();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.LoginAsync(model.ViewModelToDto());

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? Messages.Error.Auth.InvalidCredentials);
                return View(model);
            }

            // ── First login → force reset password ──
            if (result.IsFirstLogin)
            {
                TempData["InfoMessage"] = Messages.Info.InfoMessage;
                var encodedToken = Uri.EscapeDataString(result.FirstLoginToken);
                return Redirect($"/Auth/ResetPassword?token={encodedToken}");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Name, result.FullName),
                new Claim(ClaimTypes.Role, result.Role),
                new Claim("EmployeeId", result.EmployeeId.ToString()),
                new Claim("ProfilePicture", result.ProfilePicturePath ?? "")
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(30)
                        : DateTimeOffset.UtcNow.AddMinutes(30),
                    AllowRefresh = true
                });

            HttpContext.Session.SetString("UserId", result.UserId.ToString());
            HttpContext.Session.SetString("EmployeeId", result.EmployeeId.ToString());
            HttpContext.Session.SetString("FullName", result.FullName);
            HttpContext.Session.SetString("Role", result.Role);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDashboard(result.Role);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = Messages.Success.Auth.LogoutSuccess;
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.ForgotPasswordAsync(
                new ForgotPasswordDto { Email = model.Email });

            if (result.Success)
            {
                TempData["SuccessMessage"] = Messages.Success.Auth.PasswordResetSent;
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError("", result.Message ?? Messages.Error.Auth.EmailNotFound);
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = Messages.Error.Auth.InvalidResetLink;
                return RedirectToAction("Login");
            }

            var decodedToken = Uri.UnescapeDataString(token);
            var isValid = await _authService.ValidateResetTokenAsync(decodedToken);

            if (!isValid)
            {
                TempData["ErrorMessage"] = Messages.Error.Auth.InvalidToken;
                return RedirectToAction("Login");
            }

            if (TempData["InfoMessage"] != null)
                ViewData["InfoMessage"] = TempData["InfoMessage"];

            return View(new ResetPasswordViewModel { Token = decodedToken });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.ResetPasswordAsync(new ResetPasswordDto
            {
                Token = model.Token,
                NewPassword = model.NewPassword,
                ConfirmPassword = model.ConfirmPassword
            });

            if (result.Success)
            {
                TempData["SuccessMessage"] = Messages.Success.Auth.PasswordResetSuccess;
                return RedirectToAction("ResetPasswordConfirmation");
            }

            ModelState.AddModelError("", result.Message ?? Messages.Error.Auth.PasswordUpdateFailed);
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation() => View();

        [HttpGet]
        public IActionResult AccessDenied() => View();

        // ── PRIVATE ─────────────────────────────────────────────

        private IActionResult RedirectToDashboard(string? role = null)
        {
            role ??= User.FindFirst(ClaimTypes.Role)?.Value;

            return role switch
            {
                "Admin" => RedirectToAction("Index", "Hierarchy"),
                "Manager" => RedirectToAction("Dashboard", "Manager"),
                _ => RedirectToAction("Profile", "Employee")
            };
        }
    }
}