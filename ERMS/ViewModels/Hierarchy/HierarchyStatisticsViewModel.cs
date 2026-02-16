namespace ERMS.ViewModels.Hierarchy
{
    public class HierarchyStatisticsViewModel
    {
        public int TotalEmployees { get; set; }
        public int TopLevelManagers { get; set; }
        public int ManagersCount { get; set; }
        public int MaxHierarchyDepth { get; set; }
        public double AverageSpanOfControl { get; set; }
        public int EmployeesWithoutManager { get; set; }
    }
}
