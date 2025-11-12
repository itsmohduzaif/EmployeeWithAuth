using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FakeItEasy;
using FluentAssertions;
using HRManagement.Data;
using HRManagement.DTOs.AccountsDTOs;
using HRManagement.Entities;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using HRManagement.Services.Accounts;
using HRManagement.Services.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagement.Tests.Services
{
    public class AccountServiceTests
    {
        private readonly UserManager<Entities.User> _userManager;
        private readonly IJwtHandler _jwtHandler;
        private readonly IEmailService _emailService;

        public AccountServiceTests()
        {
            _userManager = A.Fake<UserManager<Entities.User>>();
            _jwtHandler = A.Fake<IJwtHandler>();
            _emailService = A.Fake<IEmailService>();
        }


        private async Task<AppDbContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();



            // Clearing the Seeding that happens
            databaseContext.Employees.RemoveRange(databaseContext.Employees);
            await databaseContext.SaveChangesAsync();


            if (await databaseContext.Employees.CountAsync() <= 0)
            {
                databaseContext.Employees.Add(
                    new Employee
                    {
                        UserName = "user1",
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = "System",
                        ModifiedBy = "System",
                        EmployeeRole = "Employee",
                        EmployeeName = "A. Gaud",
                        WorkEmail = "user1@datafirstservices.com"
                    }
                    );

                databaseContext.Users.Add(
                    new Entities.User
                    {
                        UserName = "user1",
                        Email = "user1@datafirstservices.com",
                        EmployeeName = "A. Gaud",
                        Id = Guid.NewGuid().ToString()
                    }
                );
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async Task AccountService_Login_ReturnsSuccessResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            var userForAuthentication = new UserForAuthenticationDto
            {
                WorkEmail = "user1@datafirstservices.com",
                Password = "Password@123!"
            };

            var user = new Entities.User
            {
                Email = userForAuthentication.WorkEmail,
                UserName = "user1",
                EmployeeName = "A. Gaud"
            };



            A.CallTo(() => _userManager.FindByEmailAsync(userForAuthentication.WorkEmail))
                            .Returns(Task.FromResult(user));


            A.CallTo(() => _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                            .Returns(Task.FromResult(true));


            //var rolesList = new List<string> { "Employee" };

            //A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(rolesList);


            A.CallTo(() => _userManager.GetRolesAsync(user))
                            .Returns(Task.FromResult<IList<string>>(new List<string> { "Employee" }));


            var expectedToken = "fake-jwt-token";
            A.CallTo(() => _jwtHandler.CreateToken(user, A<IList<string>>.Ignored))
                .Returns(expectedToken);


            var AccountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await AccountService.Login(userForAuthentication);


            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Login successful");
            result.Response.Should().NotBeNull();

        }



        [Fact]
        public async Task AccountService_Login_ReturnsErrorWhenWorkEmailNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            var userForAuthentication = new UserForAuthenticationDto
            {
                WorkEmail = "userxxc@datafirstservices.com",
                Password = "Password@123!"
            };

            var user = new Entities.User
            {
                Email = userForAuthentication.WorkEmail,
                UserName = "user1",
                EmployeeName = "A. Gaud"
            };


            A.CallTo(() => _userManager.FindByEmailAsync(userForAuthentication.WorkEmail))
                            .Returns(Task.FromResult<Entities.User?>(null));


            A.CallTo(() => _userManager.CheckPasswordAsync(null, userForAuthentication.Password!))
                            .Returns(Task.FromResult(false));


            //var rolesList = new List<string> { "Employee" };

            //A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(rolesList);


            A.CallTo(() => _userManager.GetRolesAsync(user))
                            .Returns(Task.FromResult<IList<string>>(new List<string> { "Employee" }));


            var expectedToken = "fake-jwt-token";
            A.CallTo(() => _jwtHandler.CreateToken(user, A<IList<string>>.Ignored))
                .Returns(expectedToken);


            var AccountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await AccountService.Login(userForAuthentication);


            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(401);
            result.Message.Should().Be("Invalid email or password");

        }



        [Fact]
        public async Task AccountService_Login_ReturnsErrorWhenPasswordIsIncorrect()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            var userForAuthentication = new UserForAuthenticationDto
            {
                WorkEmail = "user1@datafirstservices.com",
                Password = "Password@123!"
            };

            var user = new Entities.User
            {
                Email = userForAuthentication.WorkEmail,
                UserName = "user1",
                EmployeeName = "A. Gaud"
            };



            A.CallTo(() => _userManager.FindByEmailAsync(userForAuthentication.WorkEmail))
                            .Returns(Task.FromResult(user));


            A.CallTo(() => _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                            .Returns(Task.FromResult(false));


            //var rolesList = new List<string> { "Employee" };

            //A.CallTo(() => _userManager.GetRolesAsync(user)).Returns(rolesList);


            A.CallTo(() => _userManager.GetRolesAsync(user))
                            .Returns(Task.FromResult<IList<string>>(new List<string> { "Employee" }));


            var expectedToken = "fake-jwt-token";
            A.CallTo(() => _jwtHandler.CreateToken(user, A<IList<string>>.Ignored))
                .Returns(expectedToken);


            var AccountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await AccountService.Login(userForAuthentication);


            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(401);
            result.Message.Should().Be("Invalid email or password");
        }







        [Fact]
        public async Task AccountService_ForgotPasswordAsync_SendsEmailIfTheEmailExists()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var dto = new ForgotPasswordDto
            {
                WorkEmail = "user1@datafirstservices.com"
            };


            var user = new Entities.User
            {
                Email = "user1@datafirstservices.com",
                UserName = "user1",
                EmployeeName = "A. Gaud"
            };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult(user));


            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(user))
                            .Returns(Task.FromResult("fake-reset-token"));


            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();

            var AccountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await AccountService.ForgotPasswordAsync(dto);


            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("If the email exists, password reset instructions have been sent.");
        }




        [Fact]
        public async Task ForgotPasswordAsync_UserDoesNotExist_ReturnsGenericSuccessResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var email = "nonexistent@company.com";
            var dto = new ForgotPasswordDto { WorkEmail = email };

            A.CallTo(() => _userManager.FindByEmailAsync(email))
                .Returns(Task.FromResult<Entities.User?>(null));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ForgotPasswordAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("If the email exists, password reset instructions have been sent.");
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(A<Entities.User>._)).MustNotHaveHappened();
            A.CallTo(() => _emailService.SendEmail(A<string>._, A<string>._, A<string>._)).MustNotHaveHappened();
        }



        [Fact]
        public async Task ForgotPasswordAsync_EmailServiceThrowsException_ReturnsSuccessAnyway()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var email = "user1@company.com";
            var dto = new ForgotPasswordDto { WorkEmail = email };

            var user = new Entities.User { Email = email, UserName = "user1" };

            A.CallTo(() => _userManager.FindByEmailAsync(email))
                .Returns(Task.FromResult(user));

            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(user))
                .Returns(Task.FromResult("fake-reset-token"));

            // Simulate SendEmail throwing an exception
            A.CallTo(() => _emailService.SendEmail(A<string>._, A<string>._, A<string>._))
                .Throws(new Exception("SMTP failure"));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ForgotPasswordAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("If the email exists, password reset instructions have been sent.");

            // The token should still have been generated once
            A.CallTo(() => _userManager.GeneratePasswordResetTokenAsync(user))
                .MustHaveHappenedOnceExactly();

            // EmailService.SendEmail should have been called once, even though it threw
            A.CallTo(() => _emailService.SendEmail(A<string>._, A<string>._, A<string>._))
                .MustHaveHappenedOnceExactly();
        }






        




        [Fact]
        public async Task ResetPasswordAsync_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var user = new Entities.User { Email = "user@company.com", UserName = "user1" };
            var dto = new ResetPasswordDto
            {
                WorkEmail = user.Email,
                Token = "valid-token",
                NewPassword = "NewPass@123!"
            };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult(user));

            A.CallTo(() => _userManager.ResetPasswordAsync(user, A<string>._, dto.NewPassword))
                .Returns(Task.FromResult(IdentityResult.Success));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ResetPasswordAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Password reset successfully");
        }





        [Fact]
        public async Task ResetPasswordAsync_UserNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var dto = new ResetPasswordDto
            {
                WorkEmail = "missing@company.com",
                Token = "some-token",
                NewPassword = "NewPass@123!"
            };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult<Entities.User?>(null));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ResetPasswordAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("User not found");
        }


        [Fact]
        public async Task ResetPasswordAsync_ResetPasswordFails_ReturnsErrorResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var user = new Entities.User { Email = "user@company.com", UserName = "user1" };
            var dto = new ResetPasswordDto
            {
                WorkEmail = user.Email,
                Token = "invalid-token",
                NewPassword = "WeakPass"
            };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult(user));

            var identityErrors = new List<IdentityError>
            {
                new IdentityError { Description = "Invalid token." }
            };

            var identityResult = IdentityResult.Failed(identityErrors.ToArray());

            A.CallTo(() => _userManager.ResetPasswordAsync(user, A<string>._, dto.NewPassword))
                .Returns(Task.FromResult(identityResult));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ResetPasswordAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Contain("Invalid token");
        }













        //////
        ///
        /////

        ///

        ////


        [Fact]
        public async Task ChangePasswordAsync_Success_ReturnsSuccessResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var dto = new ChangePasswordDto
            {
                WorkEmail = "user1@datafirstservices.com",
                CurrentPassword = "OldPass@123!",
                NewPassword = "NewPass@123!"
            };
            string usernameFromClaim = "user1";


            var user = new Entities.User { Email = "user1@datafirstservices.com", UserName = "user1" };
            

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult(user));

            A.CallTo(() => _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword))
                .Returns(Task.FromResult(IdentityResult.Success));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ChangePasswordAsync(dto, usernameFromClaim);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Password changed successfully");
        }






        [Fact]
        public async Task ChangePasswordAsync_UserNotFound_ReturnsNotFoundResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var dto = new ChangePasswordDto
            {
                WorkEmail = "user1@datafirstservices.com",
                CurrentPassword = "OldPass@123!",
                NewPassword = "NewPass@123!"
            };
            string usernameFromClaim = "user1";

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult<Entities.User?>(null));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ChangePasswordAsync(dto, usernameFromClaim);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("User not found");
        }

        [Fact]
        public async Task ChangePasswordAsync_UsernameMismatch_ReturnsErrorResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            
            var dto = new ChangePasswordDto
            {
                WorkEmail = "user1@datafirstservices.com",
                CurrentPassword = "OldPass@123!",
                NewPassword = "NewPass@123!"
            };
            string usernameFromClaim = "user1w";

            var user = new Entities.User { Email = "user@company.com", UserName = "user1" };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult(user));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ChangePasswordAsync(dto, usernameFromClaim);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("You can only change your password only.");
        }


        [Fact]
        public async Task ChangePasswordAsync_Fails_ReturnsErrorResponse()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            
            var dto = new ChangePasswordDto
            {
                WorkEmail = "user1@datafirstservices.com",
                CurrentPassword = "OldPass@123!",
                NewPassword = "NewPass@123!"
            };
            string usernameFromClaim = "user1";

            var user = new Entities.User { Email = "user@company.com", UserName = "user1" };

            A.CallTo(() => _userManager.FindByEmailAsync(dto.WorkEmail))
                .Returns(Task.FromResult(user));

            var errors = new[] { new IdentityError { Description = "Incorrect password." } };
            A.CallTo(() => _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword))
                .Returns(Task.FromResult(IdentityResult.Failed(errors)));

            var accountService = new AccountService(_userManager, _jwtHandler, _context, _emailService);

            // Act
            var result = await accountService.ChangePasswordAsync(dto, usernameFromClaim);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Contain("Incorrect password");
        }


        











    }
}
