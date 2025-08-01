using HRManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.SeedConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Seed data for roles
            builder.HasData(
                new Role
                {
                    Id = "639de03f-7876-4fff-96ec-37f8bd3bf180",
                    Name = "Employee",
                    NormalizedName = "EMPLOYEE",
                    Description = "The Employee role for the user"
                },
                new Role
                {
                    Id = "45deb9d6-c1ae-44a6-a03b-c9a5cfc15f2f",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "The admin role for the user"
                },
                new Role
                {
                    Id = "8c7768a9-f7b3-4a0a-8b45-d74e44e367af", 
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    Description = "The Manager role for the user"
                }   
            );

        }
    }
}
