namespace ERMS.ViewModels.Role
{
    public class RoleDetailsViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int EmployeeCount { get; set; }

    }
}
