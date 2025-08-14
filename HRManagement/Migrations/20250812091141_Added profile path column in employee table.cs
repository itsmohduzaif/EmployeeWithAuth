using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRManagement.Migrations
{
    /// <inheritdoc />
    public partial class Addedprofilepathcolumninemployeetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeaveBalances");

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 5);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicturePath",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[] { "e1a1247c-7d96-4ac5-a2e4-7d5fe5fae6e7", null, "The HR role for the user", "Hr", "HR" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e1a1247c-7d96-4ac5-a2e4-7d5fe5fae6e7");

            migrationBuilder.DropColumn(
                name: "ProfilePicturePath",
                table: "Employees");

            migrationBuilder.CreateTable(
                name: "LeaveBalances",
                columns: table => new
                {
                    LeaveBalanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    LeaveTypeId = table.Column<int>(type: "int", nullable: false),
                    TotalAllocated = table.Column<int>(type: "int", nullable: false),
                    Used = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaveBalances", x => x.LeaveBalanceId);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "CreatedBy", "CreatedDate", "Email", "EmployeeRole", "FirstName", "IsActive", "LastName", "ModifiedBy", "ModifiedDate", "Phone", "Username" },
                values: new object[,]
                {
                    { 1, "System", new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "amit.sharma@datafirst.com", "Software Engineer", "Amit", true, "Sharma", "System", new DateTime(2024, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "9876543210", "amit.sharma" },
                    { 2, "System", new DateTime(2024, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "nisha.verma@datafirst.com", "QA Analyst", "Nisha", true, "Verma", "System", new DateTime(2024, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "9898989898", "nisha.verma" },
                    { 3, "System", new DateTime(2024, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "rahul.kumar@datafirst.com", "HR Executive", "Rahul", false, "Kumar", "System", new DateTime(2024, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "9123456789", "rahul.kumar" },
                    { 4, "System", new DateTime(2024, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "pooja.mehta@datafirst.com", "Project Manager", "Pooja", true, "Mehta", "System", new DateTime(2024, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "9911223344", "pooja.mehta" },
                    { 5, "System", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "vikram.singh@datafirst.com", "UI/UX Designer", "Vikram", true, "Singh", "System", new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "9001234567", "vikram.singh" }
                });

            migrationBuilder.InsertData(
                table: "LeaveBalances",
                columns: new[] { "LeaveBalanceId", "EmployeeId", "LeaveTypeId", "TotalAllocated", "Used" },
                values: new object[,]
                {
                    { 1, 1, 1, 20, 5 },
                    { 2, 1, 2, 10, 2 },
                    { 3, 1, 7, 3, 1 },
                    { 4, 2, 1, 20, 10 },
                    { 5, 2, 2, 10, 4 },
                    { 6, 2, 3, 90, 30 },
                    { 7, 3, 2, 10, 8 },
                    { 8, 3, 5, 5, 2 },
                    { 9, 4, 1, 20, 6 },
                    { 10, 4, 4, 15, 0 },
                    { 11, 4, 6, 0, 2 },
                    { 12, 5, 1, 20, 7 },
                    { 13, 5, 7, 2, 0 },
                    { 14, 5, 2, 10, 5 }
                });
        }
    }
}
