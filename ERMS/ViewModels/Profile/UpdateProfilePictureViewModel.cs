using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Profile
{
    public class UpdateProfilePictureViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please select a profile picture")]
        public IFormFile ProfilePicture { get; set; }
    }
}
