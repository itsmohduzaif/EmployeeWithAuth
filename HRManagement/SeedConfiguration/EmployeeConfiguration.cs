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
                    EmployeeName = "John Doe",
                    UserName = "johndoe",
                    WorkEmail = "admin1@datafirstservices.com",
                    PersonalPhone = "9876543210",   
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 7, 1),
                    ModifiedDate = new DateTime(2024, 7, 15),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "Admin"
                },
                new Employee
                {
                    EmployeeId = 2,
                    EmployeeName = "Jane Roe",
                    UserName = "janeroe",
                    WorkEmail = "admin2@datafirstservices.com",
                    PersonalPhone = "9876543210",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 7, 1),
                    ModifiedDate = new DateTime(2024, 7, 15),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "Admin"
                },
                new Employee
                {
                    EmployeeId = 3,
                    EmployeeName = "John Smith",
                    UserName = "johnsmith",
                    WorkEmail = "superadmin@datafirstservices.com",
                    PersonalPhone   = "9876543210",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 7, 1),
                    ModifiedDate = new DateTime(2024, 7, 15),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "Super Admin"
                },
                new Employee
                {
                    EmployeeId = 4,
                    EmployeeName = "Ankur Gaud",
                    UserName = "ankurgaud",
                    WorkEmail = "ankurgaud@datafirstservices.com",
                    PersonalPhone = "9876543210",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 7, 1),
                    ModifiedDate = new DateTime(2024, 7, 15),
                    CreatedBy = "System",
                    ModifiedBy = "System",
                    EmployeeRole = "Employee"
                }
                //new Employee
                //{
                //    EmployeeId = 5,
                //    FirstName = "Vikram",
                //    LastName = "Singh",
                //    Username = "vikram.singh",
                //    Email = "vikram.singh@datafirst.com",
                //    Phone = "9001234567",
                //    IsActive = true,
                //    CreatedDate = new DateTime(2024, 6, 1),
                //    ModifiedDate = new DateTime(2024, 6, 20),
                //    CreatedBy = "System",
                //    ModifiedBy = "System",
                //    EmployeeRole = "UI/UX Designer"
                //}
            );
        }
    }
}
