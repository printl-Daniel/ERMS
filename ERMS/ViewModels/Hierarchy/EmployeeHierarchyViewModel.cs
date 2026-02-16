using ERMS.DTOs.Hierarchy;

namespace ERMS.ViewModels.Hierarchy
{
    public class EmployeeHierarchyViewModel
    {
        public HierarchyNodeViewModel Employee { get; set; }
        public List<HierarchyNodeViewModel> ManagerChain { get; set; }
        public List<HierarchyNodeViewModel> DirectReports { get; set; }
        public EmployeeHierarchyInfoViewModel Info { get; set; }
    }
}
