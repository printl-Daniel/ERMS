using System.ComponentModel.DataAnnotations;

namespace ERMS.DTOs.Position
{
    public class CreatePositionDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Base salary is required")]
        [Range(0, 9999999.99, ErrorMessage = "Base salary must be between 0 and 9,999,999.99")]
        public decimal BaseSalary { get; set; }
    }
}
