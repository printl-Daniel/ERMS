// ResetPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Auth
{
    public class ResetPasswordViewModel : IValidatableObject
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (!NewPassword.Any(char.IsUpper))
                    yield return new ValidationResult(
                        "Password must contain at least one uppercase letter.",
                        new[] { nameof(NewPassword) });

                if (!NewPassword.Any(char.IsLower))
                    yield return new ValidationResult(
                        "Password must contain at least one lowercase letter.",
                        new[] { nameof(NewPassword) });

                if (!NewPassword.Any(char.IsDigit))
                    yield return new ValidationResult(
                        "Password must contain at least one number.",
                        new[] { nameof(NewPassword) });

                if (!NewPassword.Any(c => "!@#$%^&*()_+-=[]{}|;':\",./<>?".Contains(c)))
                    yield return new ValidationResult(
                        "Password must contain at least one special character (!@#$%...).",
                        new[] { nameof(NewPassword) });
            }
        }
    }
}