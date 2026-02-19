using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ERMS.Migrations
{
    /// <inheritdoc />
    public partial class sljsdjd : Migration
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
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
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
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    IsFirstLogin = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
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
                columns: new[] { "Id", "Address", "DateOfBirth", "DeletedAt", "DepartmentId", "Email", "FirstName", "HireDate", "IsDeleted", "LastName", "ManagerId", "PhoneNumber", "PositionId", "ProfilePicturePath", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "123 Main St, City, State 12345", new DateTime(1980, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1, "john.smith@company.com", "John", new DateTime(2020, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Smith", null, "555-0001", 1, null, 0, null },
                    { 2, "456 Oak Ave, City, State 12345", new DateTime(1985, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 2, "sarah.johnson@company.com", "Sarah", new DateTime(2020, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Johnson", 1, "555-0002", 2, null, 0, null },
                    { 3, "789 Pine Rd, City, State 12345", new DateTime(1983, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, "michael.chen@company.com", "Michael", new DateTime(2020, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Chen", 1, "555-0003", 3, null, 0, null },
                    { 4, "321 Elm St, City, State 12345", new DateTime(1987, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4, "emily.davis@company.com", "Emily", new DateTime(2020, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Davis", 1, "555-0004", 6, null, 0, null },
                    { 5, "654 Maple Dr, City, State 12345", new DateTime(1986, 7, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 5, "david.wilson@company.com", "David", new DateTime(2020, 5, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Wilson", 1, "555-0005", 8, null, 0, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "EmployeeId", "IsActive", "IsFirstLogin", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, false, "$2a$11$AQNqLKYnGield/qiZvl4I.b/iBG0JEqZc64bSPyEGQRaXShJ2b1b.", 0, null, "admin" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Address", "DateOfBirth", "DeletedAt", "DepartmentId", "Email", "FirstName", "HireDate", "IsDeleted", "LastName", "ManagerId", "PhoneNumber", "PositionId", "ProfilePicturePath", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 6, "987 Cedar Ln, City, State 12345", new DateTime(1990, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, "james.brown@company.com", "James", new DateTime(2021, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Brown", 3, "555-0006", 4, null, 0, null },
                    { 7, "147 Birch St, City, State 12345", new DateTime(1995, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 3, "lisa.garcia@company.com", "Lisa", new DateTime(2022, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Garcia", 3, "555-0007", 5, null, 0, null },
                    { 8, "258 Willow Way, City, State 12345", new DateTime(1992, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4, "robert.martinez@company.com", "Robert", new DateTime(2021, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Martinez", 4, "555-0008", 7, null, 0, null },
                    { 9, "369 Spruce Ave, City, State 12345", new DateTime(1993, 12, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 4, "jennifer.taylor@company.com", "Jennifer", new DateTime(2022, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Taylor", 4, "555-0009", 7, null, 1, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "EmployeeId", "IsActive", "IsFirstLogin", "PasswordHash", "Role", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, true, false, "$2a$11$AiKPk1mSOuunTlOiFoFBuu0buClsKdlOLdOUXI59FlLSpn2ZuWdsG", 1, null, "sarah.j" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, true, false, "$2a$11$AiKPk1mSOuunTlOiFoFBuu0buClsKdlOLdOUXI59FlLSpn2ZuWdsG", 1, null, "michael.c" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, true, false, "$2a$11$AiKPk1mSOuunTlOiFoFBuu0buClsKdlOLdOUXI59FlLSpn2ZuWdsG", 1, null, "emily.d" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, true, false, "$2a$11$7hfhstr70d1K/tgEgsk2dOe7MA0dO5UpX.ZIPVYrxWgawp0.Ho68K", 2, null, "james.b" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, true, false, "$2a$11$7hfhstr70d1K/tgEgsk2dOe7MA0dO5UpX.ZIPVYrxWgawp0.Ho68K", 2, null, "lisa.g" }
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
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

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
                name: "PasswordResetTokens");

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
