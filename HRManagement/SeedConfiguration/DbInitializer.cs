using HRManagement.Data;
using HRManagement.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HRManagement.SeedConfiguration
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            // Seed Roles if they don't exist
            //await SeedRolesAsync(roleManager);

            // Seed Users if they don't exist
            await SeedUsersAsync(userManager);
        }

        // This is not synced for description field, moreover roles are added from SeedConfiguration/RoleConfiguration.cs already
        // Will delete this
        //private static async Task SeedRolesAsync(RoleManager<Role> roleManager)
        //{
        //    string[] roleNames = { "Admin", "Manager", "Employee" };

        //    foreach (var roleName in roleNames)
        //    {
        //        var roleExist = await roleManager.RoleExistsAsync(roleName);
        //        if (!roleExist)
        //        {
        //            var role = new Role { Name = roleName };
        //            await roleManager.CreateAsync(role);
        //        }
        //    }
        //} 

        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            if (!userManager.Users.Any())
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    EmployeeName = "Admin",
                };

                var result = await userManager.CreateAsync(adminUser, "Testing32!Password"); // Testing32!Password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }

                //var managerUser = new User
                //{
                //    UserName = "manager@company.com",
                //    Email = "manager@company.com",
                //    EmployeeName = "Manager"
                //};

                //result = await userManager.CreateAsync(managerUser, "Manager123!");
                //if (result.Succeeded)
                //{
                //    await userManager.AddToRoleAsync(managerUser, "Manager");
                //}

                //var employeeUser = new User
                //{
                //    UserName = "employee@company.com",
                //    Email = "employee@company.com",
                //    EmployeeName = "Employee"
                //};

                //result = await userManager.CreateAsync(employeeUser, "Employee123!");
                //if (result.Succeeded)
                //{
                //    await userManager.AddToRoleAsync(employeeUser, "Employee");
                //}
            }
        }
    }
}
