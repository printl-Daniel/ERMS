namespace ERMS.ViewModels.Profile
{
    public class ProfileViewModel
    {
        // Display Information (Read-only)
        public int EmployeeId { get; set; }
        public string EmployeeNumber { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime HireDate { get; set; }
        public string ProfilePicturePath { get; set; }
    }
}
