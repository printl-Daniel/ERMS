using ERMS.DTOs.Hierarchy;

namespace ERMS.ViewModels.Hierarchy
{
    public class OrgChartViewModel
    {
        public List<OrgChartNodeDto> ChartData { get; set; }
        public string Title { get; set; }
        public int TotalNodes { get; set; }
    }
}
