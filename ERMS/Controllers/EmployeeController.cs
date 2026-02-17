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

        // ── GET /Employee/Edit/5 ─────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee is null)
                {
                    TempData["Error"] = "Employee not found.";
                    return RedirectToAction(nameof(Index));
                }

                var vm = new EmployeeEditViewModel
                {
                    Id = employee.Id,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    PhoneNumber = employee.PhoneNumber,
                    DepartmentId = employee.DepartmentId,
                    PositionId = employee.PositionId,
                    ManagerId = employee.ManagerId,
                    Status = employee.Status.ToString(),
                };

                await PopulateDropdownsAsync(vm);
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading employee: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ── POST /Employee/Edit/5 ────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel vm)
        {
            if (id != vm.Id)
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            // Map ViewModel → UpdateEmployeeDto (matching your exact DTO)
            var dto = new UpdateEmployeeDto
            {
                Id = vm.Id,
                FirstName = vm.FirstName.Trim(),
                LastName = vm.LastName.Trim(),
                Email = vm.Email.Trim(),
                PhoneNumber = vm.PhoneNumber?.Trim(),
                DepartmentId = vm.DepartmentId,
                PositionId = vm.PositionId,
                ManagerId = vm.ManagerId,
                Status = vm.Status,
            };

            var success = await _employeeService.UpdateEmployeeAsync(dto);

            if (!success)
            {
                ModelState.AddModelError(string.Empty,
                    "The employee could not be updated. They may no longer exist.");
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            TempData["Success"] = $"{vm.FullName} was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // ── Private: populate all three dropdowns ────────────────────────
        private async Task PopulateDropdownsAsync(EmployeeEditViewModel vm)
        {
            var departments = await _departmentRepository.GetAllAsync();
            vm.Departments = departments.Select(d =>
                new SelectListItem(d.Name, d.Id.ToString()));

            var positions = await _positionRepository.GetAllAsync();
            vm.Positions = positions.Select(p =>
                new SelectListItem(p.Title, p.Id.ToString()));

            // Exclude the employee being edited so they can't manage themselves
            vm.Managers = await _employeeRepository
                .GetManagerDropdownAsync(excludeEmployeeId: vm.Id);
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