using ERMS.Constants;
using ERMS.DTOs;
using ERMS.DTOs.Department;
using ERMS.Helpers.Mappers;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;

namespace ERMS.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _repository.GetAllActiveAsync();
            return departments.Select(d => d.ToDto());
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var department = await _repository.GetByIdActiveAsync(id);
            if (department == null)
                throw new InvalidOperationException(Messages.Error.Department.NotFound);
            return department.ToDto();
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            if (await _repository.NameExistsAsync(dto.Name))
                throw new InvalidOperationException(Messages.Error.Department.NameExists);

            var created = await _repository.CreateAsync(dto.ToEntity());
            return created.ToDto();
        }

        public async Task<DepartmentDto> UpdateAsync(UpdateDepartmentDto dto)
        {
            var department = await _repository.GetByIdActiveAsync(dto.Id);
            if (department == null)
                throw new InvalidOperationException(Messages.Error.Department.NotFound);

            if (await _repository.NameExistsAsync(dto.Name, dto.Id))
                throw new InvalidOperationException(Messages.Error.Department.NameExists);

            department.Name = dto.Name;
            department.Description = dto.Description;

            var updated = await _repository.UpdateAsync(department);
            return updated.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _repository.GetByIdActiveAsync(id);
            if (department == null)
                throw new InvalidOperationException(Messages.Error.Department.NotFound);

            if (department.Employees != null && department.Employees.Any(e => !e.IsDeleted))
                throw new InvalidOperationException(Messages.Error.Department.HasEmployees);

            return await _repository.SoftDeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _repository.ExistsAsync(id);
    }
}