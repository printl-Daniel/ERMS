// DTOs/Hierarchy/EmployeeHierarchyDto.cs
namespace ERMS.DTOs.Hierarchy
{
    public class EmployeeHierarchyDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PositionTitle { get; set; }
        public string DepartmentName { get; set; }
        public string ProfilePicturePath { get; set; }
        public int? ManagerId { get; set; }
        public string ManagerName { get; set; }
        public int SubordinateCount { get; set; }
        public int Level { get; set; }
        public List<EmployeeHierarchyDto> Subordinates { get; set; } = new List<EmployeeHierarchyDto>();
    }
}