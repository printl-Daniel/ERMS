namespace ERMS.ViewModels.Department
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int EmployeeCount { get; set; }
        public bool IsInUse => EmployeeCount > 0;
    }
}