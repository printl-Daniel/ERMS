using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERMS.Migrations
{
    /// <inheritdoc />
    public partial class sdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "DeletedAt", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, null, "C-Level executives", false, "Executive" },
                    { 2, null, "HR and talent management", false, "Human Resources" },
                    { 3, null, "Software development and IT", false, "Engineering" },
                    { 4, null, "Sales and business development", false, "Sales" },
                    { 5, null, "Marketing and communications", false, "Marketing" }
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "BaseSalary", "DeletedAt", "Description", "IsDeleted", "Title" },
                values: new object[,]
                {
                    { 1, 250000m, null, "Chief Executive Officer", false, "CEO" },
                    { 2, 90000m, null, "Human Resources Manager", false, "HR Manager" },
                    { 3, 120000m, null, "Engineering Team Lead", false, "Engineering Manager" },
                    { 4, 100000m, null, "Senior Software Developer", false, "Senior Developer" },
                    { 5, 65000m, null, "Junior Software Developer", false, "Junior Developer" },
                    { 6, 95000m, null, "Sales Team Manager", false, "Sales Manager" },
                    { 7, 55000m, null, "Sales Rep", false, "Sales Representative" },
                    { 8, 90000m, null, "Marketing Team Manager", false, "Marketing Manager" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "Email", "FirstName", "HireDate", "LastName", "ManagerId", "PhoneNumber", "PositionId", "Status" },
                values: new object[,]
                {
                    { 1, 1, "john.smith@company.com", "John", new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smith", null, "555-0001", 1, 0 },
                    { 2, 2, "sarah.johnson@company.com", "Sarah", new DateTime(2020, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Johnson", 1, "555-0002", 2, 0 },
                    { 3, 3, "michael.chen@company.com", "Michael", new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chen", 1, "555-0003", 3, 0 },
                    { 4, 4, "emily.davis@company.com", "Emily", new DateTime(2020, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Davis", 1, "555-0004", 6, 0 },
                    { 5, 5, "david.wilson@company.com", "David", new DateTime(2020, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Wilson", 1, "555-0005", 8, 0 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "EmployeeId", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, "admin123", 0, "admin" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "DepartmentId", "Email", "FirstName", "HireDate", "LastName", "ManagerId", "PhoneNumber", "PositionId", "Status" },
                values: new object[,]
                {
                    { 6, 3, "james.brown@company.com", "James", new DateTime(2021, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brown", 3, "555-0006", 4, 0 },
                    { 7, 3, "lisa.garcia@company.com", "Lisa", new DateTime(2022, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Garcia", 3, "555-0007", 5, 0 },
                    { 8, 4, "robert.martinez@company.com", "Robert", new DateTime(2021, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Martinez", 4, "555-0008", 7, 0 },
                    { 9, 4, "jennifer.taylor@company.com", "Jennifer", new DateTime(2022, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Taylor", 4, "555-0009", 7, 1 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "EmployeeId", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, "manager123", 1, "sarah.j" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, true, "manager123", 1, "michael.c" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, true, "manager123", 1, "emily.d" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, true, "employee123", 2, "james.b" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, true, "employee123", 2, "lisa.g" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_PositionId",
                table: "Employees",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmployeeId",
                table: "Users",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Positions");
        }
    }
}
