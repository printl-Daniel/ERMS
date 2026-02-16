// Services/HierarchyService.cs
using ERMS.DTOs.Hierarchy;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;

namespace ERMS.Services
{
    public class HierarchyService : IHierarchyService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public HierarchyService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<EmployeeHierarchyDto>> GetOrganizationHierarchyAsync()
        {
            var topLevelEmployees = await _employeeRepository.GetTopLevelEmployeesAsync();
            var hierarchyList = new List<EmployeeHierarchyDto>();

            foreach (var employee in topLevelEmployees)
            {
                var dto = await BuildHierarchyDto(employee, 0);
                hierarchyList.Add(dto);
            }

            return hierarchyList;
        }

        public async Task<EmployeeHierarchyDto> GetEmployeeHierarchyAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetByIdWithHierarchyAsync(employeeId);
            if (employee == null) return null;

            return await BuildHierarchyDto(employee, 0);
        }

        public async Task<List<OrgChartNodeDto>> GetOrgChartDataAsync()
        {
            var allEmployees = await _employeeRepository.GetAllWithRelationsAsync();
            var chartNodes = new List<OrgChartNodeDto>();

            foreach (var employee in allEmployees)
            {
                var node = new OrgChartNodeDto
                {
                    Id = employee.Id,
                    Name = employee.FullName,
                    Title = employee.Position?.Title,
                    Department = employee.Department?.Name,
                    Email = employee.Email,
                    Phone = employee.PhoneNumber,
                    ImageUrl = employee.ProfilePicturePath ?? "/images/default-avatar.png",
                    ParentId = employee.ManagerId
                };
                chartNodes.Add(node);
            }

            // Build tree structure
            var rootNodes = chartNodes.Where(n => n.ParentId == null).ToList();
            foreach (var root in rootNodes)
            {
                BuildOrgChartTree(root, chartNodes);
            }

            return rootNodes;
        }

        public async Task<List<EmployeeHierarchyDto>> GetDepartmentHierarchyAsync(int departmentId)
        {
            var departmentEmployees = await _employeeRepository.GetByDepartmentAsync(departmentId);
            var topLevel = departmentEmployees.Where(e => e.ManagerId == null ||
                           !departmentEmployees.Any(d => d.Id == e.ManagerId));

            var hierarchyList = new List<EmployeeHierarchyDto>();
            foreach (var employee in topLevel)
            {
                var dto = await BuildHierarchyDto(employee, 0);
                hierarchyList.Add(dto);
            }

            return hierarchyList;
        }

        public async Task<List<EmployeeHierarchyDto>> GetManagerChainAsync(int employeeId)
        {
            var chain = await _employeeRepository.GetEmployeeChainAsync(employeeId);
            var chainList = new List<EmployeeHierarchyDto>();

            int level = 0;
            foreach (var employee in chain)
            {
                chainList.Add(MapToHierarchyDto(employee, level++));
            }

            return chainList;
        }

        public async Task<Dictionary<string, object>> GetHierarchyStatisticsAsync()
        {
            var allEmployees = await _employeeRepository.GetAllWithRelationsAsync();
            var topLevel = await _employeeRepository.GetTopLevelEmployeesAsync();

            var stats = new Dictionary<string, object>
            {
                ["TotalEmployees"] = allEmployees.Count(),
                ["TopLevelManagers"] = topLevel.Count(),
                ["AverageSpanOfControl"] = CalculateAverageSpanOfControl(allEmployees),
                ["MaxHierarchyDepth"] = CalculateMaxDepth(topLevel.ToList()),
                ["EmployeesWithoutManager"] = allEmployees.Count(e => e.ManagerId == null),
                ["ManagersCount"] = allEmployees.Count(e => e.Subordinates.Any())
            };

            return stats;
        }

        // Private helper methods
        private async Task<EmployeeHierarchyDto> BuildHierarchyDto(Employee employee, int level)
        {
            var dto = MapToHierarchyDto(employee, level);

            if (employee.Subordinates != null && employee.Subordinates.Any())
            {
                foreach (var subordinate in employee.Subordinates)
                {
                    var subEmployee = await _employeeRepository.GetByIdWithHierarchyAsync(subordinate.Id);
                    if (subEmployee != null)
                    {
                        var subDto = await BuildHierarchyDto(subEmployee, level + 1);
                        dto.Subordinates.Add(subDto);
                    }
                }
            }

            return dto;
        }

        private EmployeeHierarchyDto MapToHierarchyDto(Employee employee, int level)
        {
            return new EmployeeHierarchyDto
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                PositionTitle = employee.Position?.Title,
                DepartmentName = employee.Department?.Name,
                ProfilePicturePath = employee.ProfilePicturePath ?? "/images/default-avatar.png",
                ManagerId = employee.ManagerId,
                ManagerName = employee.Manager?.FullName,
                SubordinateCount = employee.Subordinates?.Count ?? 0,
                Level = level
            };
        }

        private void BuildOrgChartTree(OrgChartNodeDto parent, List<OrgChartNodeDto> allNodes)
        {
            var children = allNodes.Where(n => n.ParentId == parent.Id).ToList();
            parent.Children = children;

            foreach (var child in children)
            {
                BuildOrgChartTree(child, allNodes);
            }
        }

        private double CalculateAverageSpanOfControl(IEnumerable<Employee> employees)
        {
            var managers = employees.Where(e => e.Subordinates != null && e.Subordinates.Any()).ToList();
            if (!managers.Any()) return 0;

            return Math.Round((double)managers.Sum(m => m.Subordinates.Count) / managers.Count, 2);
        }

        private int CalculateMaxDepth(List<Employee> topLevel)
        {
            int maxDepth = 0;
            foreach (var employee in topLevel)
            {
                int depth = CalculateEmployeeDepth(employee, 1);
                maxDepth = Math.Max(maxDepth, depth);
            }
            return maxDepth;
        }

        private int CalculateEmployeeDepth(Employee employee, int currentDepth)
        {
            if (employee.Subordinates == null || !employee.Subordinates.Any())
                return currentDepth;

            int maxSubDepth = currentDepth;
            foreach (var subordinate in employee.Subordinates)
            {
                int subDepth = CalculateEmployeeDepth(subordinate, currentDepth + 1);
                maxSubDepth = Math.Max(maxSubDepth, subDepth);
            }

            return maxSubDepth;
        }
    }
}