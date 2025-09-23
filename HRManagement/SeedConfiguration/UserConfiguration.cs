//using HRManagement.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace HRManagement.SeedConfiguration
//{
//    public class UserConfiguration : IEntityTypeConfiguration<User>
//    {
//        public void Configure(EntityTypeBuilder<User> builder)
//        {
//            // Seed data for roles
//            builder.HasData(
//                new User
//                {
//                    Id = "22deb9d6-c1ae-4fff-96ec-37f8bd3bf971",
//                    EmployeeName = "THE ADMIN",
//                    UserName = "admin",
//                    NormalizedUserName = "ADMIN",
//                    Email = "imuzaifmohd2@gmail.com",
//                    NormalizedEmail = "ADMIN@GMAIL.COM",
//                    EmailConfirmed = false,
//                    PhoneNumber = "9876543210",
//                    PhoneNumberConfirmed = false

//                }
//            );
//        }
//    }
//}
