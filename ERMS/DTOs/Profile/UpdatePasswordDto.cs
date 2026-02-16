namespace ERMS.DTOs.Profile
{
    public class UpdatePasswordDto
    {
        public int EmployeeId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

}
