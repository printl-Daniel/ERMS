using ERMS.DTOs;
using ERMS.DTOs.Position;
using ERMS.ViewModels.Position;

namespace ERMS.Helpers
{
    public static class PositionMapper
    {
        public static PositionListViewModel ToListViewModel(this PositionDto dto)
        {
            return new PositionListViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                BaseSalary = dto.BaseSalary,
                EmployeeCount = dto.EmployeeCount
            };
        }

        public static EditPositionViewModel ToEditViewModel(this PositionDto dto)
        {
            return new EditPositionViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                BaseSalary = dto.BaseSalary
            };
        }

        public static DeletePositionViewModel ToDeleteViewModel(this PositionDto dto)
        {
            return new DeletePositionViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                BaseSalary = dto.BaseSalary,
                EmployeeCount = dto.EmployeeCount
            };
        }

        public static CreatePositionDto ToCreateDto(this CreatePositionViewModel viewModel)
        {
            return new CreatePositionDto
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                BaseSalary = viewModel.BaseSalary
            };
        }

        public static UpdatePositionDto ToUpdateDto(this EditPositionViewModel viewModel)
        {
            return new UpdatePositionDto
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                BaseSalary = viewModel.BaseSalary
            };
        }
    }
}