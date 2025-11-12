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
                var adminUser1 = new User
                {
                    UserName = "johndoe",
                    Email = "admin1@datafirstservices.com",
                    EmployeeName = "John Doe",
                };

                var result = await userManager.CreateAsync(adminUser1, "Testing32!Password"); // Testing32!Password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser1, "Admin");
                }

                // Admin User 2
                var adminUser2 = new User
                {
                    UserName = "janeroe",
                    Email = "admin2@datafirstservices.com",
                    EmployeeName = "Jane Roe",
                };

                result = await userManager.CreateAsync(adminUser2, "Testing32!Password"); // Testing32!Password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser2, "Admin");
                }



                // Super Admin User
                var superAdminUser = new User
                {
                    UserName = "johnsmith",
                    Email = "superadmin@datafirstservices.com",
                    EmployeeName = "John Smith",
                };

                result = await userManager.CreateAsync(superAdminUser, "Testing32!Password"); // Testing32!Password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superAdminUser, "Super Admin");
                }


                // Normal Employee Users
                var employeeUser1 = new User
                {
                    UserName = "ankurgaud",
                    Email = "ankurgaud@datafirstservices.com",
                    EmployeeName = "Ankur Gaud",
                };

                result = await userManager.CreateAsync(employeeUser1, "Testing32!Password"); // Testing32!Password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employeeUser1, "Employee");
                }

            }
        }
    }
}
