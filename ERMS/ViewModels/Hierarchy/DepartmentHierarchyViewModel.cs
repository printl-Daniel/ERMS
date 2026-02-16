using ERMS.DTOs.Hierarchy;

namespace ERMS.ViewModels.Hierarchy
{
    public class DepartmentHierarchyViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public List<HierarchyNodeViewModel> Hierarchy { get; set; }
        public DepartmentHierarchyStatisticsViewModel Statistics { get; set; }
    }
}
