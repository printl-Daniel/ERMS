using ERMS.DTOs;
using ERMS.DTOs.Position;
using ERMS.Services.Interfaces;
using ERMS.ViewModels;
using ERMS.ViewModels.Position;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Controllers
{
    public class PositionController : Controller
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        // GET: Positions
        public async Task<IActionResult> Index()
        {
            try
            {
                var positions = await _positionService.GetAllPositionsAsync();
                var viewModel = positions.Select(p => new PositionListViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    BaseSalary = p.BaseSalary,
                    EmployeeCount = p.EmployeeCount
                }).ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading positions.";
                return View(new List<PositionListViewModel>());
            }
        }

        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var position = await _positionService.GetPositionByIdAsync(id);
                var viewModel = new PositionListViewModel
                {
                    Id = position.Id,
                    Title = position.Title,
                    Description = position.Description,
                    BaseSalary = position.BaseSalary,
                    EmployeeCount = position.EmployeeCount
                };

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Positions/Create
        public IActionResult Create()
        {
            return View(new CreatePositionViewModel());
        }

        // POST: Positions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePositionViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var createDto = new CreatePositionDto
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    BaseSalary = viewModel.BaseSalary
                };

                await _positionService.CreatePositionAsync(createDto);
                TempData["Success"] = $"Position '{viewModel.Title}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(viewModel);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while creating the position.");
                return View(viewModel);
            }
        }

        // GET: Positions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var position = await _positionService.GetPositionByIdAsync(id);
                var viewModel = new EditPositionViewModel
                {
                    Id = position.Id,
                    Title = position.Title,
                    Description = position.Description,
                    BaseSalary = position.BaseSalary
                };

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Positions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPositionViewModel viewModel)
        {
            if (id != viewModel.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var updateDto = new UpdatePositionDto
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    BaseSalary = viewModel.BaseSalary
                };

                await _positionService.UpdatePositionAsync(id, updateDto);
                TempData["Success"] = $"Position '{viewModel.Title}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(viewModel);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred while updating the position.");
                return View(viewModel);
            }
        }

        // GET: Positions/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var position = await _positionService.GetPositionByIdAsync(id);
                var viewModel = new DeletePositionViewModel
                {
                    Id = position.Id,
                    Title = position.Title,
                    Description = position.Description,
                    BaseSalary = position.BaseSalary,
                    EmployeeCount = position.EmployeeCount
                };

                return View(viewModel);
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _positionService.DeletePositionAsync(id);
                TempData["Success"] = "Position deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Position not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "An error occurred while deleting the position.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}