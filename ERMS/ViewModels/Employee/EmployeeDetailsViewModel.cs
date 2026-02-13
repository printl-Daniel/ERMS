namespace ERMS.ViewModels.Employee
{
    public class EmployeeDetailsViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Manager { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public List<SubordinateViewModel> Subordinates { get; set; }
    }
}