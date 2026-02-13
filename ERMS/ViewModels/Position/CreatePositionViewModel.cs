using System.ComponentModel.DataAnnotations;

namespace ERMS.ViewModels.Position
{
    public class CreatePositionViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Position Title")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Base salary is required")]
        [Range(0, 9999999.99, ErrorMessage = "Base salary must be between 0 and 9,999,999.99")]
        [Display(Name = "Base Salary")]
        [DataType(DataType.Currency)]
        public decimal BaseSalary { get; set; }
    }
}
