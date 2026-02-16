namespace ERMS.DTOs.Profile
{
     public class UpdatePersonalInfoDto
    {
        public int EmployeeId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

}
