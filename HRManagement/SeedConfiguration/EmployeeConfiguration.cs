using HRManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.SeedConfiguration
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasData(
                new Employee
                {
                    EmployeeId = 1,
                    FirstName = "Amit",
                    LastName = "Sharma",
                    Username = "amit.sharma",
                    Email = "amit.sharma@datafirst.com",
                    Phone = "9876543210",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 7, 1),
                    ModifiedDate = new DateTime(2024, 7, 15),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "Software Engineer"
                },
                new Employee
                {
                    EmployeeId = 2,
                    FirstName = "Nisha",
                    LastName = "Verma",
                    Username = "nisha.verma",
                    Email = "nisha.verma@datafirst.com",
                    Phone = "9898989898",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 6, 18),
                    ModifiedDate = new DateTime(2024, 6, 25),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "QA Analyst"
                },
                new Employee
                {
                    EmployeeId = 3,
                    FirstName = "Rahul",
                    LastName = "Kumar",
                    Username = "rahul.kumar",
                    Email = "rahul.kumar@datafirst.com",
                    Phone = "9123456789",
                    IsActive = false,
                    CreatedDate = new DateTime(2024, 5, 10),
                    ModifiedDate = new DateTime(2024, 5, 20),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "HR Executive"
                },
                new Employee
                {
                    EmployeeId = 4,
                    FirstName = "Pooja",
                    LastName = "Mehta",
                    Username = "pooja.mehta",
                    Email = "pooja.mehta@datafirst.com",
                    Phone = "9911223344",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 7, 5),
                    ModifiedDate = new DateTime(2024, 7, 10),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "Project Manager"
                },
                new Employee
                {
                    EmployeeId = 5,
                    FirstName = "Vikram",
                    LastName = "Singh",
                    Username = "vikram.singh",
                    Email = "vikram.singh@datafirst.com",
                    Phone = "9001234567",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 6, 1),
                    ModifiedDate = new DateTime(2024, 6, 20),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "UI/UX Designer"
                }
            );
        }
    }
}
