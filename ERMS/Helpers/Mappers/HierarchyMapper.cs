// Extensions/HierarchyMappingExtensions.cs
using ERMS.DTOs.Hierarchy;
using ERMS.ViewModels.Hierarchy;

namespace ERMS.Extensions
{
    public static class HierarchyMappingExtensions
    {
        public static HierarchyNodeViewModel ToViewModel(this EmployeeHierarchyDto dto)
        {
            if (dto == null) return null;

            return new HierarchyNodeViewModel
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PositionTitle = dto.PositionTitle,
                DepartmentName = dto.DepartmentName,
                ProfilePicturePath = dto.ProfilePicturePath,
                SubordinateCount = dto.SubordinateCount,
                Level = dto.Level,
                Subordinates = dto.Subordinates?.Select(s => s.ToViewModel()).ToList() ?? new List<HierarchyNodeViewModel>()
            };
        }

        public static List<HierarchyNodeViewModel> ToViewModelList(this List<EmployeeHierarchyDto> dtos)
        {
            if (dtos == null) return new List<HierarchyNodeViewModel>();
            return dtos.Select(dto => dto.ToViewModel()).ToList();
        }
    }
}