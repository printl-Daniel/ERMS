using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Employee
{
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        [MaxLength(100)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Hire date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Department is required")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [Display(Name = "Position")]
        public int PositionId { get; set; }

        [Display(Name = "Manager")]
        public int? ManagerId { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "User Role")]
        public string Role { get; set; }

        // Select lists for dropdowns - NO VALIDATION ON THESE!
        public IEnumerable<SelectListItem>? Departments { get; set; }
        public IEnumerable<SelectListItem>? Positions { get; set; }
        public IEnumerable<SelectListItem>? Managers { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}