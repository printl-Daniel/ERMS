using ERMS.DTOs.Employee;
using ERMS.Models;
using ERMS.Repositories.Interfaces;
using ERMS.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using static ERMS.Enums.EmployeeEnum;

namespace ERMS.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(MapToResponseDto);
        }

        public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee != null ? MapToResponseDto(employee) : null;
        }

        public async Task<CreateEmployeeResponseDto> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            try
            {
                // Validate email doesn't already exist
                if (await _employeeRepository.EmailExistsAsync(dto.Email))
                {
                    return new CreateEmployeeResponseDto
                    {
                        Success = false,
                        Message = "An employee with this email already exists"
                    };
                }

                // Create employee
                var employee = new Employee
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    HireDate = dto.HireDate,
                    DepartmentId = dto.DepartmentId,
                    PositionId = dto.PositionId,
                    ManagerId = dto.ManagerId,
                    Status = EmployeeStatus.Active
                };

                var createdEmployee = await _employeeRepository.CreateAsync(employee);

                // Generate username from email (take part before @)
                var username = GenerateUsername(dto.Email);

                // Generate random password
                var generatedPassword = GenerateRandomPassword();

                // Parse role
                UserRole userRole;
                if (!Enum.TryParse(dto.Role, out userRole))
                {
                    userRole = UserRole.Employee; // Default to Employee if parsing fails
                }

                // Hash the password
                var hashedPassword = HashPassword(generatedPassword);

                // Create user account
                var user = new User
                {
                    Username = username,
                    PasswordHash = hashedPassword, // Now properly hashed!
                    Role = userRole,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    EmployeeId = createdEmployee.Id
                };

                await _userRepository.CreateAsync(user);

                // Determine if employee is a manager
                bool isManager = userRole == UserRole.Manager || userRole == UserRole.Admin;

                // Send email with credentials
                var emailSent = await _emailService.SendPasswordEmailAsync(
                    dto.Email,
                    $"{dto.FirstName} {dto.LastName}",
                    username,
                    generatedPassword, // Send plain password in email (user will change it)
                    isManager
                );

                return new CreateEmployeeResponseDto
                {
                    Success = true,
                    Message = "Employee created successfully",
                    EmployeeId = createdEmployee.Id,
                    Username = username,
                    GeneratedPassword = generatedPassword, // Return plain password to show admin
                    EmailSent = emailSent
                };
            }
            catch (Exception ex)
            {
                return new CreateEmployeeResponseDto
                {
                    Success = false,
                    Message = $"Error creating employee: {ex.Message}"
                };
            }
        }

        public async Task<bool> UpdateEmployeeAsync(UpdateEmployeeDto dto)
        {
            var employee = await _employeeRepository.GetByIdAsync(dto.Id);
            if (employee == null)
                return false;

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;
            employee.Email = dto.Email;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.DepartmentId = dto.DepartmentId;
            employee.PositionId = dto.PositionId;
            employee.ManagerId = dto.ManagerId;

            EmployeeStatus status;
            if (Enum.TryParse(dto.Status, out status))
            {
                employee.Status = status;
            }

            await _employeeRepository.UpdateAsync(employee);
            return true;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await _employeeRepository.DeleteAsync(id);
        }

        // Helper Methods
        private string GenerateUsername(string email)
        {
            // Extract part before @ and make lowercase
            var username = email.Split('@')[0].ToLower();

            // Remove any dots or special characters
            username = username.Replace(".", "").Replace("-", "");

            return username;
        }

        private string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%";
            var random = new Random();
            var password = new StringBuilder();

            // Ensure at least one of each type
            password.Append((char)random.Next('a', 'z' + 1)); // lowercase
            password.Append((char)random.Next('A', 'Z' + 1)); // uppercase
            password.Append((char)random.Next('0', '9' + 1)); // digit
            password.Append("!@#"[random.Next(3)]); // special char

            // Fill the rest randomly
            for (int i = 4; i < length; i++)
            {
                password.Append(validChars[random.Next(validChars.Length)]);
            }

            // Shuffle the password
            return new string(password.ToString().ToCharArray().OrderBy(x => random.Next()).ToArray());
        }

        // IMPORTANT: Hash password using SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private EmployeeResponseDto MapToResponseDto(Employee employee)
        {
            return new EmployeeResponseDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                HireDate = employee.HireDate,
                Status = employee.Status.ToString(),
                DepartmentName = employee.Department?.Name,
                PositionTitle = employee.Position?.Title,
                ManagerName = employee.Manager?.FullName,
                Username = employee.User?.Username,
                Role = employee.User?.Role.ToString()
            };
        }


    }
}