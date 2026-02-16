namespace ERMS.ViewModels.Hierarchy
{
    public class EmployeeHierarchyInfoViewModel
    {
        public int TotalSubordinates { get; set; }
        public int DirectReports { get; set; }
        public int HierarchyLevel { get; set; }
        public bool IsTopLevel { get; set; }
        public bool HasSubordinates { get; set; }
    }
}
