using ERMS.Constants.ERMS.Constants;
using ERMS.DTOs.Employee;
using ERMS.Helpers.Mappers;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERMS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeController(
            IEmployeeService employeeService,
            IDepartmentRepository departmentRepository,
            IPositionRepository positionRepository,
            IEmployeeRepository employeeRepository)
        {
            _employeeService = employeeService;
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _employeeRepository = employeeRepository;
        }

        // Check if user is admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // Populate dropdowns for employee forms
        private async Task PopulateDropdowns(CreateEmployeeViewModel model, int? excludeEmployeeId = null)
        {
            model.Departments = await _departmentRepository.GetDepartmentDropdownAsync();
            model.Positions = await _positionRepository.GetPositionDropdownAsync();
            model.Managers = await _employeeRepository.GetManagerDropdownAsync(excludeEmployeeId);
        }

        // GET: Employee/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            var employees = await _employeeService.GetAllEmployeesAsync();
            var viewModel = employees.ToListViewModels();

            return View(viewModel);
        }

        // GET: Employee/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            var viewModel = new CreateEmployeeViewModel();
            await PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var dto = model.ModelToDto();
            var result = await _employeeService.CreateEmployeeAsync(dto);

            if (result.Success)
            {
                var resultViewModel = result.ToResultViewModel(model);
                return View("CreateResult", resultViewModel);
            }

            ModelState.AddModelError("", result.Message);
            await PopulateDropdowns(model);
            return View(model);
        }

        // GET: Employee/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            var employeeEntity = await _employeeRepository.GetByIdAsync(id);
            var viewModel = employee.ToDetailsViewModel(employeeEntity);

            return View(viewModel);
        }

        // GET: Employee/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = Messages.Success.EmployeeDeleted;
            }
            else
            {
                TempData["ErrorMessage"] = Messages.Error.EmployeeDeleteFailed;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}