using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Department
{
    public class EditDepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Department Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
