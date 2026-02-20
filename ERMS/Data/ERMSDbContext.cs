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
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Query Filters ────────────────────────────────────────────
            modelBuilder.Entity<Employee>()
                .HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.Employee.IsDeleted);

            modelBuilder.Entity<PasswordResetToken>()
                .HasQueryFilter(t => !t.User.Employee.IsDeleted);

            // ── Relationships ────────────────────────────────────────────
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Manager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Unique Constraints ───────────────────────────────────────
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(t => t.Token)
                .IsUnique();

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(t => t.UserId);

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            ///{"admin":"$2a$11$AQNqLKYnGield/qiZvl4I.b/iBG0JEqZc64bSPyEGQRaXShJ2b1b.","manager":"$2a$11$AiKPk1mSOuunTlOiFoFBuu0buClsKdlOLdOUXI59FlLSpn2ZuWdsG","employee":"$2a$11$7hfhstr70d1K/tgEgsk2dOe7MA0dO5UpX.ZIPVYrxWgawp0.Ho68K"}
            const string adminHash = "$2a$11$AQNqLKYnGield/qiZvl4I.b/iBG0JEqZc64bSPyEGQRaXShJ2b1b.";
            const string managerHash = "$2a$11$AiKPk1mSOuunTlOiFoFBuu0buClsKdlOLdOUXI59FlLSpn2ZuWdsG";
            const string employeeHash = "$2a$11$7hfhstr70d1K/tgEgsk2dOe7MA0dO5UpX.ZIPVYrxWgawp0.Ho68K";

            // ── Departments ──────────────────────────────────────────────
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "Executive", Description = "C-Level executives" },
                new Department { Id = 2, Name = "Human Resources", Description = "HR and talent management" },
                new Department { Id = 3, Name = "Engineering", Description = "Software development and IT" },
                new Department { Id = 4, Name = "Sales", Description = "Sales and business development" },
                new Department { Id = 5, Name = "Marketing", Description = "Marketing and communications" }
            );

            // ── Positions ────────────────────────────────────────────────
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
            modelBuilder.Entity<Employee>().HasData(
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
                    ManagerId = null,
                    Address = "123 Main St, City, State 12345",
                    DateOfBirth = new DateTime(1980, 5, 15),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
                },
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
                    ManagerId = 1,
                    Address = "456 Oak Ave, City, State 12345",
                    DateOfBirth = new DateTime(1985, 8, 22),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
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
                    ManagerId = 1,
                    Address = "789 Pine Rd, City, State 12345",
                    DateOfBirth = new DateTime(1983, 3, 10),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
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
                    ManagerId = 1,
                    Address = "321 Elm St, City, State 12345",
                    DateOfBirth = new DateTime(1987, 11, 30),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
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
                    ManagerId = 1,
                    Address = "654 Maple Dr, City, State 12345",
                    DateOfBirth = new DateTime(1986, 7, 18),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
                },
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
                    ManagerId = 3,
                    Address = "987 Cedar Ln, City, State 12345",
                    DateOfBirth = new DateTime(1990, 2, 14),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
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
                    ManagerId = 3,
                    Address = "147 Birch St, City, State 12345",
                    DateOfBirth = new DateTime(1995, 9, 5),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
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
                    ManagerId = 4,
                    Address = "258 Willow Way, City, State 12345",
                    DateOfBirth = new DateTime(1992, 4, 25),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
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
                    ManagerId = 4,
                    Address = "369 Spruce Ave, City, State 12345",
                    DateOfBirth = new DateTime(1993, 12, 8),
                    IsDeleted = false,
                    CreatedAt = new DateTime(2024, 1, 1)        // ← added
                }
            );


            // ── Users ────────────────────────────────────────────────────
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", IsFirstLogin = false, PasswordHash = adminHash, Role = UserRole.Admin, IsActive = true, CreatedAt = new DateTime(2024, 1, 1), EmployeeId = 1 },
                new User { Id = 2, Username = "sarah.j", IsFirstLogin = false, PasswordHash = managerHash, Role = UserRole.Manager, IsActive = true, CreatedAt = new DateTime(2024, 1, 1), EmployeeId = 2 },
                new User { Id = 3, Username = "michael.c", IsFirstLogin = false, PasswordHash = managerHash, Role = UserRole.Manager, IsActive = true, CreatedAt = new DateTime(2024, 1, 1), EmployeeId = 3 },
                new User { Id = 4, Username = "emily.d", IsFirstLogin = false, PasswordHash = managerHash, Role = UserRole.Manager, IsActive = true, CreatedAt = new DateTime(2024, 1, 1), EmployeeId = 4 },
                new User { Id = 5, Username = "james.b", IsFirstLogin = false, PasswordHash = employeeHash, Role = UserRole.Employee, IsActive = true, CreatedAt = new DateTime(2024, 1, 1), EmployeeId = 6 },
                new User { Id = 6, Username = "lisa.g", IsFirstLogin = false, PasswordHash = employeeHash, Role = UserRole.Employee, IsActive = true, CreatedAt = new DateTime(2024, 1, 1), EmployeeId = 7 }
            );
        }
    }
}