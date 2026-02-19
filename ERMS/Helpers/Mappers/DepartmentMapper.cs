using ERMS.DTOs;
using ERMS.DTOs.Department;
using ERMS.Models;
using ERMS.ViewModels.Department;

namespace ERMS.Helpers.Mappers
{
    public static class DepartmentMapper
    {
        // ── SERVICE MAPPERS ───────────────────────────────────────────────────

        // Department → DepartmentDto
        public static DepartmentDto ToDto(this Department department) =>
            new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };

        // CreateDepartmentDto → Department
        public static Department ToEntity(this CreateDepartmentDto dto) =>
            new Department
            {
                Name = dto.Name,
                Description = dto.Description,
                IsDeleted = false
            };

        // ── CONTROLLER MAPPERS ────────────────────────────────────────────────

        // DepartmentDto → DepartmentViewModel
        public static DepartmentViewModel ToViewModel(this DepartmentDto dto) =>
            new DepartmentViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

        // DepartmentDto → EditDepartmentViewModel
        public static EditDepartmentViewModel ToEditViewModel(this DepartmentDto dto) =>
            new EditDepartmentViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

        // DepartmentDto → DeleteDepartmentViewModel
        public static DeleteDepartmentViewModel ToDeleteViewModel(this DepartmentDto dto) =>
            new DeleteDepartmentViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };

        // CreateDepartmentViewModel → CreateDepartmentDto
        public static CreateDepartmentDto ToCreateDto(this CreateDepartmentViewModel viewModel) =>
            new CreateDepartmentDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description
            };

        // EditDepartmentViewModel → UpdateDepartmentDto
        public static UpdateDepartmentDto ToUpdateDto(this EditDepartmentViewModel viewModel) =>
            new UpdateDepartmentDto
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description
            };
    }
}