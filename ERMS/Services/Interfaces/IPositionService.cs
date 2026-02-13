using ERMS.DTOs;
using ERMS.DTOs.Position;

namespace ERMS.Services.Interfaces
{
    public interface IPositionService
    {
        Task<IEnumerable<PositionDto>> GetAllPositionsAsync();
        Task<PositionDto> GetPositionByIdAsync(int id);
        Task<PositionDto> CreatePositionAsync(CreatePositionDto createDto);
        Task<PositionDto> UpdatePositionAsync(int id, UpdatePositionDto updateDto);
        Task<bool> DeletePositionAsync(int id);
    }
}