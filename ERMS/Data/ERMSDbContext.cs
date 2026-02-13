using ERMS.Models;
using Microsoft.EntityFrameworkCore;
using static ERMS.Enums.EmployeeEnum;
namespace ERMS.Data
{

    public class ERMSDbContext : DbContext
    {
        public ERMSDbContext(DbContextOptions<ERMSDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure self-referencing relationship for Employee hierarchy
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure one-to-one relationship between Employee and User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraints
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Executive", Description = "C-Level executives" },
                new Department { Id = 2, Name = "Human Resources", Description = "HR and talent management" },
                new Department { Id = 3, Name = "Engineering", Description = "Software development and IT" },
                new Department { Id = 4, Name = "Sales", Description = "Sales and business development" },
                new Department { Id = 5, Name = "Marketing", Description = "Marketing and communications" }
            );

            // Seed Positions
            modelBuilder.Entity<Position>().HasData(
                new Position { Id = 1, Title = "CEO", Description = "Chief Executive Officer", BaseSalary = 250000m },
                new Position { Id = 2, Title = "HR Manager", Description = "Human Resources Manager", BaseSalary = 90000m },
                new Position { Id = 3, Title = "Engineering Manager", Description = "Engineering Team Lead", BaseSalary = 120000m },
                new Position { Id = 4, Title = "Senior Developer", Description = "Senior Software Developer", BaseSalary = 100000m },
                new Position { Id = 5, Title = "Junior Developer", Description = "Junior Software Developer", BaseSalary = 65000m },
                new Position { Id = 6, Title = "Sales Manager", Description = "Sales Team Manager", BaseSalary = 95000m },
                new Position { Id = 7, Title = "Sales Representative", Description = "Sales Rep", BaseSalary = 55000m },
                new Position { Id = 8, Title = "Marketing Manager", Description = "Marketing Team Manager", BaseSalary = 90000m }
            );

            // Seed Employees (with hierarchy)
            modelBuilder.Entity<Employee>().HasData(
                // CEO - No manager
                new Employee
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@company.com",
                    PhoneNumber = "555-0001",
                    HireDate = new DateTime(2020, 1, 15),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 1,
                    PositionId = 1,
                    ManagerId = null
                },

                // Department Managers - Report to CEO
                new Employee
                {
                    Id = 2,
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah.johnson@company.com",
                    PhoneNumber = "555-0002",
                    HireDate = new DateTime(2020, 3, 10),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 2,
                    PositionId = 2,
                    ManagerId = 1
                },
                new Employee
                {
                    Id = 3,
                    FirstName = "Michael",
                    LastName = "Chen",
                    Email = "michael.chen@company.com",
                    PhoneNumber = "555-0003",
                    HireDate = new DateTime(2020, 2, 20),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 3,
                    PositionId = 3,
                    ManagerId = 1
                },
                new Employee
                {
                    Id = 4,
                    FirstName = "Emily",
                    LastName = "Davis",
                    Email = "emily.davis@company.com",
                    PhoneNumber = "555-0004",
                    HireDate = new DateTime(2020, 4, 5),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 4,
                    PositionId = 6,
                    ManagerId = 1
                },
                new Employee
                {
                    Id = 5,
                    FirstName = "David",
                    LastName = "Wilson",
                    Email = "david.wilson@company.com",
                    PhoneNumber = "555-0005",
                    HireDate = new DateTime(2020, 5, 12),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 5,
                    PositionId = 8,
                    ManagerId = 1
                },

                // Team Members - Report to Department Managers
                new Employee
                {
                    Id = 6,
                    FirstName = "James",
                    LastName = "Brown",
                    Email = "james.brown@company.com",
                    PhoneNumber = "555-0006",
                    HireDate = new DateTime(2021, 6, 1),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 3,
                    PositionId = 4,
                    ManagerId = 3
                },
                new Employee
                {
                    Id = 7,
                    FirstName = "Lisa",
                    LastName = "Garcia",
                    Email = "lisa.garcia@company.com",
                    PhoneNumber = "555-0007",
                    HireDate = new DateTime(2022, 1, 15),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 3,
                    PositionId = 5,
                    ManagerId = 3
                },
                new Employee
                {
                    Id = 8,
                    FirstName = "Robert",
                    LastName = "Martinez",
                    Email = "robert.martinez@company.com",
                    PhoneNumber = "555-0008",
                    HireDate = new DateTime(2021, 8, 20),
                    Status = EmployeeStatus.Active,
                    DepartmentId = 4,
                    PositionId = 7,
                    ManagerId = 4
                },
                new Employee
                {
                    Id = 9,
                    FirstName = "Jennifer",
                    LastName = "Taylor",
                    Email = "jennifer.taylor@company.com",
                    PhoneNumber = "555-0009",
                    HireDate = new DateTime(2022, 3, 10),
                    Status = EmployeeStatus.OnLeave,
                    DepartmentId = 4,
                    PositionId = 7,
                    ManagerId = 4
                }
            );

            // Seed Users (role-based login)
            // Note: In production, use proper password hashing (e.g., BCrypt)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "admin123", // DEMO ONLY - Hash this in production!
                    Role = UserRole.Admin,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1),

                    EmployeeId = 1
                },
                new User
                {
                    Id = 2,
                    Username = "sarah.j",
                    PasswordHash = "manager123",
                    Role = UserRole.Manager,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1),

                    EmployeeId = 2
                },
                new User
                {
                    Id = 3,
                    Username = "michael.c",
                    PasswordHash = "manager123",
                    Role = UserRole.Manager,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1),

                    EmployeeId = 3
                },
                new User
                {
                    Id = 4,
                    Username = "emily.d",
                    PasswordHash = "manager123",
                    Role = UserRole.Manager,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1),

                    EmployeeId = 4
                },
                new User
                {
                    Id = 5,
                    Username = "james.b",
                    PasswordHash = "employee123",
                    Role = UserRole.Employee,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1),

                    EmployeeId = 6
                },
                new User
                {
                    Id = 6,
                    Username = "lisa.g",
                    PasswordHash = "employee123",
                    Role = UserRole.Employee,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1),
                    EmployeeId = 7
                }
            );
        }
    }
}