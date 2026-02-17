using ERMS.DTOs;
using ERMS.DTOs.Department;
using ERMS.Models;
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
            return departments.Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name,
                Description = d.Description
            });
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            var department = await _repository.GetByIdActiveAsync(id);
            if (department == null)
                return null;

            return new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description
            };
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            if (await _repository.NameExistsAsync(dto.Name))
                throw new InvalidOperationException("A department with this name already exists");

            var department = new Department
            {
                Name = dto.Name,
                Description = dto.Description,
                IsDeleted = false
            };

            var created = await _repository.CreateAsync(department);

            return new DepartmentDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description
            };
        }

        public async Task<DepartmentDto> UpdateAsync(UpdateDepartmentDto dto)
        {
            var department = await _repository.GetByIdActiveAsync(dto.Id);
            if (department == null)
                throw new InvalidOperationException("Department not found");

            // Check if name already exists (excluding current department)
            if (await _repository.NameExistsAsync(dto.Name, dto.Id))
                throw new InvalidOperationException("A department with this name already exists");

            department.Name = dto.Name;
            department.Description = dto.Description;

            var updated = await _repository.UpdateAsync(department);

            return new DepartmentDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.SoftDeleteAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}