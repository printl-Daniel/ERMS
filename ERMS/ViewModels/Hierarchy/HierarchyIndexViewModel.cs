using ERMS.DTOs.Hierarchy;

namespace ERMS.ViewModels.Hierarchy
{
    public class HierarchyIndexViewModel
    {
        public List<HierarchyNodeViewModel> Hierarchy { get; set; }
        public HierarchyStatisticsViewModel Statistics { get; set; }
    }
}
