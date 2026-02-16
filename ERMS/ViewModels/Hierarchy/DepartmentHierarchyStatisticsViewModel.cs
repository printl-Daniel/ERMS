namespace ERMS.ViewModels.Hierarchy
{
    public class DepartmentHierarchyStatisticsViewModel
    {
        public int TotalEmployees { get; set; }
        public int ManagersCount { get; set; }
        public int TopLevelCount { get; set; }
        public double AverageSpanOfControl { get; set; }

    }
}