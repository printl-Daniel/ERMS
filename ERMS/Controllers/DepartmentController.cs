using ERMS.Helpers;
using ERMS.Helpers.Mappers;
using ERMS.Services.Interfaces;
using ERMS.ViewModels.Department;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var departments = await _departmentService.GetAllAsync();
            var viewModels = departments.Select(d => d.ToViewModel());

            return View(viewModels);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDepartmentViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _departmentService.CreateAsync(model.ToCreateDto());
                TempData["SuccessMessage"] = "Department created successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department.ToEditViewModel());
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditDepartmentViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _departmentService.UpdateAsync(model.ToUpdateDto());
                TempData["SuccessMessage"] = "Department updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound();

            return View(department.ToDeleteViewModel());
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var result = await _departmentService.DeleteAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Department deleted successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete department";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}