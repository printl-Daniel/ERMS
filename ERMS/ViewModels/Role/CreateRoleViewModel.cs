using ERMS.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Role
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage = "Role name is required")]
        [StringLength(100, ErrorMessage = "Role name cannot exceed 100 characters")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public IEnumerable<SelectListItem> Roles => Enum.GetValues(typeof(EmployeeEnum.UserRole))
            .Cast<EmployeeEnum.UserRole>()
            .Select(r => new SelectListItem
            {
                Value = r.ToString(),
                Text = r.ToString()
            });
    }
}
