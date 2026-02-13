using ERMS.DTOs;
using ERMS.DTOs.Position;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;

namespace ERMS.Services.Implementations
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;

        public PositionService(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        public async Task<IEnumerable<PositionDto>> GetAllPositionsAsync()
        {
            var positions = await _positionRepository.GetAllActiveAsync();
            return positions.Select(p => MapToDto(p));
        }

        public async Task<PositionDto> GetPositionByIdAsync(int id)
        {
            var position = await _positionRepository.GetActiveByIdAsync(id);
            if (position == null)
                throw new KeyNotFoundException($"Position with ID {id} not found");

            return MapToDto(position);
        }

        public async Task<PositionDto> CreatePositionAsync(CreatePositionDto createDto)
        {
            // Check if title already exists
            if (await _positionRepository.TitleExistsAsync(createDto.Title))
                throw new InvalidOperationException($"Position with title '{createDto.Title}' already exists");

            var position = new Position
            {
                Title = createDto.Title,
                Description = createDto.Description,
                BaseSalary = createDto.BaseSalary,
                IsDeleted = false
            };

            var createdPosition = await _positionRepository.CreateAsync(position);
            return MapToDto(createdPosition);
        }

        public async Task<PositionDto> UpdatePositionAsync(int id, UpdatePositionDto updateDto)
        {
            var position = await _positionRepository.GetActiveByIdAsync(id);
            if (position == null)
                throw new KeyNotFoundException($"Position with ID {id} not found");

            // Check if title already exists (excluding current position)
            if (await _positionRepository.TitleExistsAsync(updateDto.Title, id))
                throw new InvalidOperationException($"Position with title '{updateDto.Title}' already exists");

            position.Title = updateDto.Title;
            position.Description = updateDto.Description;
            position.BaseSalary = updateDto.BaseSalary;

            var updatedPosition = await _positionRepository.UpdateAsync(position);
            return MapToDto(updatedPosition);
        }

        public async Task<bool> DeletePositionAsync(int id)
        {
            var position = await _positionRepository.GetActiveByIdAsync(id);
            if (position == null)
                throw new KeyNotFoundException($"Position with ID {id} not found");

            // Check if position has active employees
            if (position.Employees != null && position.Employees.Any())
                throw new InvalidOperationException($"Cannot delete position '{position.Title}' because it has {position.Employees.Count} employee(s) assigned to it");

            return await _positionRepository.SoftDeleteAsync(id);
        }

        private PositionDto MapToDto(Position position)
        {
            return new PositionDto
            {
                Id = position.Id,
                Title = position.Title,
                Description = position.Description,
                BaseSalary = position.BaseSalary,
                EmployeeCount = position.Employees?.Count ?? 0
            };
        }
    }
}