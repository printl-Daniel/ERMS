using ERMS.Models;

namespace ERMS.Repositories.Interfaces
{
    public interface IPositionRepository
    {
        Task<IEnumerable<Position>> GetAllAsync();
        Task<IEnumerable<Position>> GetAllActiveAsync();
        Task<Position> GetByIdAsync(int id);
        Task<Position> GetActiveByIdAsync(int id);
        Task<Position> CreateAsync(Position position);
        Task<Position> UpdateAsync(Position position);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> TitleExistsAsync(string title, int? excludeId = null);
    }
}