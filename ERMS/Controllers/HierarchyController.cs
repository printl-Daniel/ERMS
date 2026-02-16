// Controllers/HierarchyController.cs
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Hierarchy;
using ERMS.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERMS.DTOs.Hierarchy;
using ERMS.Repositories.Interfaces;

namespace ERMS.Controllers
{
    public class HierarchyController : Controller
    {
        private readonly IHierarchyService _hierarchyService;
        private readonly IDepartmentRepository _departmentRepository;

        public HierarchyController(
            IHierarchyService hierarchyService,
            IDepartmentRepository departmentRepository)
        {
            _hierarchyService = hierarchyService;
            _departmentRepository = departmentRepository;
        }

        // GET: /Hierarchy/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var hierarchyDto = await _hierarchyService.GetOrganizationHierarchyAsync();
                var stats = await _hierarchyService.GetHierarchyStatisticsAsync();

                var viewModel = new HierarchyIndexViewModel
                {
                    Hierarchy = hierarchyDto.ToViewModelList(),
                    Statistics = new HierarchyStatisticsViewModel
                    {
                        TotalEmployees = Convert.ToInt32(stats["TotalEmployees"]),
                        TopLevelManagers = Convert.ToInt32(stats["TopLevelManagers"]),
                        ManagersCount = Convert.ToInt32(stats["ManagersCount"]),
                        MaxHierarchyDepth = Convert.ToInt32(stats["MaxHierarchyDepth"]),
                        AverageSpanOfControl = Convert.ToDouble(stats["AverageSpanOfControl"]),
                        EmployeesWithoutManager = Convert.ToInt32(stats["EmployeesWithoutManager"])
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading hierarchy: {ex.Message}";
                return View(new HierarchyIndexViewModel
                {
                    Hierarchy = new List<HierarchyNodeViewModel>(),
                    Statistics = new HierarchyStatisticsViewModel()
                });
            }
        }

        // GET: /Hierarchy/OrgChart
        public async Task<IActionResult> OrgChart()
        {
            try
            {
                var chartData = await _hierarchyService.GetOrgChartDataAsync();

                var viewModel = new OrgChartViewModel
                {
                    ChartData = chartData,
                    Title = "Organization Chart",
                    TotalNodes = CountNodes(chartData)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading org chart: {ex.Message}";
                return View(new OrgChartViewModel
                {
                    ChartData = new List<OrgChartNodeDto>(),
                    Title = "Organization Chart",
                    TotalNodes = 0
                });
            }
        }

        // GET: /Hierarchy/Employee/5
        public async Task<IActionResult> Employee(int id)
        {
            try
            {
                var employeeDto = await _hierarchyService.GetEmployeeHierarchyAsync(id);
                if (employeeDto == null)
                {
                    TempData["Error"] = "Employee not found.";
                    return RedirectToAction(nameof(Index));
                }

                var managerChainDto = await _hierarchyService.GetManagerChainAsync(id);

                // Convert DTOs to ViewModels
                var employeeViewModel = employeeDto.ToViewModel();
                var managerChainViewModel = managerChainDto.ToViewModelList();
                var directReportsViewModel = employeeDto.Subordinates.ToViewModelList();

                var viewModel = new EmployeeHierarchyViewModel
                {
                    Employee = employeeViewModel,
                    ManagerChain = managerChainViewModel,
                    DirectReports = directReportsViewModel,
                    Info = new EmployeeHierarchyInfoViewModel
                    {
                        TotalSubordinates = CountAllSubordinates(employeeDto),
                        DirectReports = employeeDto.SubordinateCount,
                        HierarchyLevel = employeeDto.Level,
                        IsTopLevel = employeeDto.ManagerId == null,
                        HasSubordinates = employeeDto.SubordinateCount > 0
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading employee hierarchy: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Hierarchy/Department/5
        public async Task<IActionResult> Department(int id)
        {
            try
            {
                var department = await _departmentRepository.GetByIdAsync(id);
                if (department == null)
                {
                    TempData["Error"] = "Department not found.";
                    return RedirectToAction(nameof(Index));
                }

                var departmentHierarchyDto = await _hierarchyService.GetDepartmentHierarchyAsync(id);

                var viewModel = new DepartmentHierarchyViewModel
                {
                    DepartmentId = id,
                    DepartmentName = department.Name,
                    Hierarchy = departmentHierarchyDto.ToViewModelList(),
                    Statistics = CalculateDepartmentStatistics(departmentHierarchyDto)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading department hierarchy: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // API endpoint for dynamic loading
        [HttpGet]
        public async Task<IActionResult> GetOrgChartData()
        {
            try
            {
                var chartData = await _hierarchyService.GetOrgChartDataAsync();
                return Json(new { success = true, data = chartData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeSubordinates(int id)
        {
            try
            {
                var employeeDto = await _hierarchyService.GetEmployeeHierarchyAsync(id);
                var subordinatesViewModel = employeeDto?.Subordinates.ToViewModelList() ?? new List<HierarchyNodeViewModel>();
                return Json(new { success = true, data = subordinatesViewModel });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // Private helper methods
        private int CountNodes(List<OrgChartNodeDto> nodes)
        {
            int count = nodes.Count;
            foreach (var node in nodes)
            {
                count += CountNodes(node.Children);
            }
            return count;
        }

        private int CountAllSubordinates(EmployeeHierarchyDto employee)
        {
            int count = employee.SubordinateCount;
            foreach (var subordinate in employee.Subordinates)
            {
                count += CountAllSubordinates(subordinate);
            }
            return count;
        }

        private DepartmentHierarchyStatisticsViewModel CalculateDepartmentStatistics(List<EmployeeHierarchyDto> hierarchy)
        {
            var allEmployees = GetAllEmployeesFromHierarchy(hierarchy);

            return new DepartmentHierarchyStatisticsViewModel
            {
                TotalEmployees = allEmployees.Count,
                ManagersCount = allEmployees.Count(e => e.SubordinateCount > 0),
                TopLevelCount = hierarchy.Count,
                AverageSpanOfControl = allEmployees.Any(e => e.SubordinateCount > 0)
                    ? Math.Round(allEmployees.Where(e => e.SubordinateCount > 0)
                        .Average(e => e.SubordinateCount), 2)
                    : 0
            };
        }

        private List<EmployeeHierarchyDto> GetAllEmployeesFromHierarchy(List<EmployeeHierarchyDto> hierarchy)
        {
            var allEmployees = new List<EmployeeHierarchyDto>();
            foreach (var employee in hierarchy)
            {
                allEmployees.Add(employee);
                if (employee.Subordinates != null && employee.Subordinates.Any())
                {
                    allEmployees.AddRange(GetAllEmployeesFromHierarchy(employee.Subordinates));
                }
            }
            return allEmployees;
        }
    }
}