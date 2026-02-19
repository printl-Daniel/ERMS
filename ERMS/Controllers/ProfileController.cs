using ERMS.Helpers.Mappers;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Profile;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(IProfileService profileService, IWebHostEnvironment webHostEnvironment)
        {
            _profileService = profileService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var employeeIdString = HttpContext.Session.GetString("EmployeeId");
            if (string.IsNullOrEmpty(employeeIdString))
                return RedirectToAction("Login", "Auth");

            var profileDto = await _profileService.GetProfileAsync(int.Parse(employeeIdString));
            if (profileDto == null)
                return NotFound();

            return View(profileDto.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePersonalInfo(UpdatePersonalInfoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                return RedirectToAction("Profile");
            }

            var result = await _profileService.UpdatePersonalInfoAsync(model.ToDto());
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the errors and try again.";
                return RedirectToAction("Profile");
            }

            var employeeIdString = HttpContext.Session.GetString("EmployeeId");
            var result = await _profileService.UpdatePasswordAsync(model.ToDto(int.Parse(employeeIdString)));
            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureViewModel model)
        {
            if (!ModelState.IsValid || model.ProfilePicture == null)
            {
                TempData["Error"] = "Please select a valid image file.";
                return RedirectToAction("Profile");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.ProfilePicture.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["Error"] = "Only image files (jpg, jpeg, png, gif) are allowed.";
                return RedirectToAction("Profile");
            }

            if (model.ProfilePicture.Length > 5 * 1024 * 1024)
            {
                TempData["Error"] = "File size must not exceed 5MB.";
                return RedirectToAction("Profile");
            }

            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "profiles");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{model.EmployeeId}_{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await model.ProfilePicture.CopyToAsync(fileStream);

                var result = await _profileService.UpdateProfilePictureAsync(
                    model.ToDto($"/uploads/profiles/{uniqueFileName}"));

                TempData[result.Success ? "Success" : "Error"] = result.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error uploading file: {ex.Message}";
            }

            return RedirectToAction("Profile");
        }
    }
}