using ERMS.DTOs;
using ERMS.DTOs.Department;
using ERMS.ViewModels.Department;

namespace ERMS.Helpers.Mappers
{
    public static class DepartmentMapper
    {
        public static DepartmentViewModel ToViewModel(this DepartmentDto dto)
        {
            return new DepartmentViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static EditDepartmentViewModel ToEditViewModel(this DepartmentDto dto)
        {
            return new EditDepartmentViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static DeleteDepartmentViewModel ToDeleteViewModel(this DepartmentDto dto)
        {
            return new DeleteDepartmentViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description
            };
        }

        public static CreateDepartmentDto ToCreateDto(this CreateDepartmentViewModel viewModel)
        {
            return new CreateDepartmentDto
            {
                Name = viewModel.Name,
                Description = viewModel.Description
            };
        }

        public static UpdateDepartmentDto ToUpdateDto(this EditDepartmentViewModel viewModel)
        {
            return new UpdateDepartmentDto
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description
            };
        }
    }
}