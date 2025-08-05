using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRManagement.Migrations
{
    /// <inheritdoc />
    public partial class initialv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
