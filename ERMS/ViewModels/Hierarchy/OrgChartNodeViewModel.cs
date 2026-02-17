using ERMS.DTOs.Hierarchy;

namespace ERMS.ViewModels.Hierarchy
{
    public class OrgChartNodeViewModel
    {
        public OrgChartNodeDto Node  { get; set; } = null!;
        public int Depth { get; set; } = 0;
    }
}
