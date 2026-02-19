// ForgotPasswordViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(256, ErrorMessage = "Email address is too long")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}