// Services/Interfaces/IHierarchyService.cs
using ERMS.DTOs.Hierarchy;

namespace ERMS.Services.Interfaces
{
    public interface IHierarchyService
    {
        Task<List<EmployeeHierarchyDto>> GetOrganizationHierarchyAsync();
        Task<EmployeeHierarchyDto> GetEmployeeHierarchyAsync(int employeeId);
        Task<List<OrgChartNodeDto>> GetOrgChartDataAsync();
        Task<List<EmployeeHierarchyDto>> GetDepartmentHierarchyAsync(int departmentId);
        Task<List<EmployeeHierarchyDto>> GetManagerChainAsync(int employeeId);
        Task<Dictionary<string, object>> GetHierarchyStatisticsAsync();
    }
}