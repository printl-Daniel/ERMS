using ERMS.Constants;
using ERMS.DTOs.Employee;
using ERMS.Helpers.Mappers;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Employee;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Employee/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees.ToListViewModels());
        }

        // GET: Employee/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateEmployeeViewModel();
            await _employeeService.PopulateDropdowns(viewModel);
            return View(viewModel);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await _employeeService.PopulateDropdowns(model);
                return View(model);
            }

            var dto = model.ModelToDto();
            var result = await _employeeService.CreateEmployeeAsync(dto);

            if (result.Success)
                return View("CreateResult", result.ToResultViewModel(model));

            ModelState.AddModelError("", result.Message);
            await _employeeService.PopulateDropdowns(model);
            return View(model);
        }

        // GET: Employee/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _employeeService.GetEmployeeForEditAsync(id);
            if (dto is null)
            {
                TempData["Error"] = Messages.Error.Employee.NotFound;
                return RedirectToAction(nameof(Index));
            }

            var vm = dto.ToEditViewModel();
            await _employeeService.PopulateDropdowns(vm, excludeEmployeeId: vm.Id);
            return View(vm);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EmployeeEditViewModel vm)
        {
            if (id != vm.Id)
            {
                TempData["Error"] = Messages.Error.Employee.InvalidRequest;
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await _employeeService.PopulateDropdowns(vm, excludeEmployeeId: vm.Id);
                return View(vm);
            }

            var success = await _employeeService.UpdateEmployeeAsync(vm.ToUpdateDto());

            if (!success)
            {
                ModelState.AddModelError(string.Empty, Messages.Error.Employee.UpdateFailed);
                await _employeeService.PopulateDropdowns(vm, excludeEmployeeId: vm.Id);
                return View(vm);
            }

            TempData["Success"] = vm.FullName + Messages.Success.Employee.Updated;
            return RedirectToAction(nameof(Index));
        }

        // GET: Employee/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var viewModel = await _employeeService.GetEmployeeDetailsAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        // GET: Employee/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var viewModel = await _employeeService.GetEmployeeForDeleteAsync(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);

            TempData[result ? "SuccessMessage" : "ErrorMessage"] = result
                ? Messages.Success.Employee.Deleted
                : Messages.Error.Employee.DeleteFailed;

            return RedirectToAction(nameof(Index));
        }
    }
}