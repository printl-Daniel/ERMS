    using ERMS.Models;

    namespace ERMS.Repositories.Interfaces
    {

        public interface IUserRepository
        {
            Task<User> GetByIdAsync(int id);
            Task<User> GetByEmployeeIdAsync(int employeeId);
            Task<User> CreateAsync(User user);
            Task<User> UpdateAsync(User user);
        }
    }
