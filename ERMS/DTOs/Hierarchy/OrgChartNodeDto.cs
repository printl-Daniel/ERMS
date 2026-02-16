// DTOs/Hierarchy/OrgChartNodeDto.cs
namespace ERMS.DTOs.Hierarchy
{
    public class OrgChartNodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ImageUrl { get; set; }
        public int? ParentId { get; set; }
        public List<OrgChartNodeDto> Children { get; set; } = new List<OrgChartNodeDto>();
    }
}