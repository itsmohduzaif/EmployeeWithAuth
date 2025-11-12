using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using FakeItEasy;
using FluentAssertions;
using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Entities;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using HRManagement.Services.BlobStorage;
using HRManagement.Services.Employees;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using Microsoft.Graph.UsersWithUserPrincipalName;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagement.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly UserManager<Entities.User> _userManager;
        private readonly IJwtHandler _jwtHandler;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public EmployeeServiceTests()
        {

            _userManager = A.Fake<UserManager<Entities.User>>();
            _jwtHandler = A.Fake<IJwtHandler>();
            _blobStorageService = A.Fake<IBlobStorageService>();
            _configuration = A.Fake<IConfiguration>();
            _mapper = A.Fake<IMapper>();
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


        // GetAllEmployees
        [Fact]
        public async Task EmployeeService_GetAllEmployees_ReturnsEmployees()
        {
            //Arrange
            var _context = await GetDatabaseContext();


            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);

            //Act
            var result = await EmployeeService.GetAllEmployees();

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            ((List<Employee>)result.Response).Should().HaveCount(1);

            var returnEmployees = (List<Employee>)result.Response;
            returnEmployees[0].UserName.Should().Be("user1");
            returnEmployees[0].WorkEmail.Should().Be("user1@datafirstservices.com");
        }
        [Fact]
        public async Task EmployeeService_GetAllEmployees_ReturnsEmptyList()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            _context.Employees.RemoveRange(_context.Employees);
            await _context.SaveChangesAsync();

            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);

            //Act
            var result = await EmployeeService.GetAllEmployees();

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            ((List<Employee>)result.Response).Should().HaveCount(0);
        }


        // GetEmployeeById
        [Fact]
        public async Task EmployeeService_GetEmployeeById_ReturnsEmployee()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var existingEmployee = await _context.Employees.FirstAsync();
            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);
            // Act
            var result = await EmployeeService.GetEmployeeById(existingEmployee.EmployeeId);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            var returnedEmployee = (Employee)result.Response;
            returnedEmployee.EmployeeId.Should().Be(existingEmployee.EmployeeId);
            returnedEmployee.UserName.Should().Be(existingEmployee.UserName);
        }

        [Fact]
        public async Task EmployeeService_GetEmployeeById_ReturnsNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);
            var nonExistentEmployeeId = 9999; // Assuming this ID does not exist
            // Act
            var result = await EmployeeService.GetEmployeeById(nonExistentEmployeeId);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Response.Should().BeNull();
        }



        // SignUpAsAnEmployee
        [Fact]
        public async Task EmployeeService_SignUpAsAnEmployee_ReturnsNewCreatedEmployee()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            //var employeeDto = A.Fake<SignUpAsAnEmployeeDTO>();            // This way the properties will be null
            // creates a fake object with all null properties, so your method will fail at the email validation stage. You need to manually populate it with valid data.


            // So manually creating the DTO with valid data
            var employeeDto = new SignUpAsAnEmployeeDTO
            {
                EmployeeName = "Test Employee",
                WorkEmail = "testemployee@datafirstservices.com",
                UserName = "testemployee",
                PersonalPhone = "1234567890",
                Password = "Password@12345!",
                IsActive = true
            };


            


            A.CallTo(() => _userManager.CreateAsync(A<Entities.User>.Ignored, A<string>.Ignored))
                            .Returns(Task.FromResult(IdentityResult.Success));

            // IdentityResult.Success short hand for new IdentityResult { Succeeded = true }

            // IdentityResult.Success is a predefined static instance that already means { Succeeded = true, Errors = [] }
            // We can’t create it manually because Succeeded has a protected setter, and using the built-in instance ensures consistency with ASP.NET Identity.



            // Task.FromResult is a helper method that creates a Task that’s already completed — with a specific result.
            // So instead of running something asynchronously, it’s a shortcut that says: “Pretend we already finished this async operation and here’s the result.”



            A.CallTo(() => _userManager.AddToRoleAsync(A<Entities.User>.Ignored, "Employee"))
                               .Returns(Task.FromResult(IdentityResult.Success)); // Return a Task<IdentityResult> with IdentityResult.Success


            
            var mappedEmployee = new Employee
            {
                EmployeeName = employeeDto.EmployeeName,
                UserName = employeeDto.UserName,
                WorkEmail = employeeDto.WorkEmail,
                PersonalPhone = "1234567890",
                IsActive = true
            };


            A.CallTo(() => _mapper.Map<Employee>(employeeDto)).Returns(mappedEmployee);






            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);


            // Act

            var result = await EmployeeService.SignUpAsAnEmployee(employeeDto);


            // Assert

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            result.Message.Should().Be("Employee created successfully");


            var createdEmployee = (Employee)result.Response;
            createdEmployee.EmployeeName.Should().Be(employeeDto.EmployeeName);
            createdEmployee.WorkEmail.Should().Be(employeeDto.WorkEmail);



            // Verify the employee was saved to DB
            var employeeInDb = await _context.Employees.FirstOrDefaultAsync(e => e.WorkEmail == employeeDto.WorkEmail);
            employeeInDb.Should().NotBeNull();
            employeeDto.EmployeeName.Should().Be(employeeDto.EmployeeName);


            // ---- Verify expected interactions ----
            A.CallTo(() => _userManager.CreateAsync(A<Entities.User>.Ignored, A<string>.Ignored))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _userManager.AddToRoleAsync(A<Entities.User>.Ignored, "Employee"))
                .MustHaveHappenedOnceExactly();


        }

        [Fact]
        public async Task EmployeeService_SignUpAsAnEmployee_ReturnsErrorWhenEmailNotValid()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            var employeeDto = new SignUpAsAnEmployeeDTO
            {
                EmployeeName = "Test Employee",
                WorkEmail = "testemployee@xyz.com",
                UserName = "testemployee",
                PersonalPhone = "1234567890",
                Password = "Password@12345!",
                IsActive = true
            };

            A.CallTo(() => _userManager.CreateAsync(A<Entities.User>.Ignored, A<string>.Ignored))
                            .Returns(Task.FromResult(IdentityResult.Success));

            
            A.CallTo(() => _userManager.AddToRoleAsync(A<Entities.User>.Ignored, "Employee"))
                               .Returns(Task.FromResult(IdentityResult.Success)); // Return a Task<IdentityResult> with IdentityResult.Success

            var mappedEmployee = new Employee
            {
                EmployeeName = employeeDto.EmployeeName,
                UserName = employeeDto.UserName,
                WorkEmail = employeeDto.WorkEmail,
                PersonalPhone = "1234567890",
                IsActive = true
            };


            A.CallTo(() => _mapper.Map<Employee>(employeeDto)).Returns(mappedEmployee);






            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);


            // Act

            var result = await EmployeeService.SignUpAsAnEmployee(employeeDto);


            // Assert

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            

        }

        [Fact]
        public async Task EmployeeService_SignUpAsAnEmployee_ReturnsErrorWhenUserRegestrationFails()    
        {
            // Arrange
            var _context = await GetDatabaseContext();

            var employeeDto = new SignUpAsAnEmployeeDTO
            {
                EmployeeName = "Test Employee",
                WorkEmail = "testemployee@datafirstservices.com",
                UserName = "testemployee",
                PersonalPhone = "1234567890",
                Password = "Password@12345!",
                IsActive = true
            };

            A.CallTo(() => _userManager.CreateAsync(A<Entities.User>.Ignored, A<string>.Ignored))
                            .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User creation failed due to some error" })));

            
            A.CallTo(() => _userManager.AddToRoleAsync(A<Entities.User>.Ignored, "Employee"))
                               .Returns(Task.FromResult(IdentityResult.Success)); // Return a Task<IdentityResult> with IdentityResult.Success

            var mappedEmployee = new Employee
            {
                EmployeeName = employeeDto.EmployeeName,
                UserName = employeeDto.UserName,
                WorkEmail = employeeDto.WorkEmail,
                PersonalPhone = "1234567890",
                IsActive = true
            };


            A.CallTo(() => _mapper.Map<Employee>(employeeDto)).Returns(mappedEmployee);






            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);


            // Act

            var result = await EmployeeService.SignUpAsAnEmployee(employeeDto);


            // Assert

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("User registration failed: User creation failed due to some error");

        }

        [Fact]
        public async Task EmployeeService_UpdateProfileAsync_UpdatesProfileWithValidInput()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            string usernameFromClaim = "user1";

            var profileUpdateDto = new EmployeeProfileUpdateDTO
            {
                EmployeeName = "Alison Timberlee",
                Status = "Active",
                EmploymentType = "Full-Time",
                WorkEmail = "user1@datafirstservices.com",
                UserName = "user1",
                PersonalPhone = "1234567890"
            };

            var user = new Entities.User
            {
                UserName = "user1",
                Email = "user1@datafirstservices.com",
                EmployeeName = "A. Gaud",
            };

            // Mock UserManager behavior
            A.CallTo(() => _userManager.FindByNameAsync(A<string>.Ignored))
                .Returns(user);

            A.CallTo(() => _userManager.UpdateAsync(A<Entities.User>.Ignored))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Mock AutoMapper mapping behavior
            A.CallTo(() => _mapper.Map(A<EmployeeProfileUpdateDTO>.Ignored, A<Employee>.Ignored))
                .Invokes((EmployeeProfileUpdateDTO src, Employee dest) =>
                {
                    dest.EmployeeName = src.EmployeeName;
                    dest.UserName = src.UserName;
                    dest.WorkEmail = src.WorkEmail;
                    dest.Status = src.Status;
                    dest.EmploymentType = src.EmploymentType;
                });

            var employeeService = new EmployeeService(
                _context,
                _userManager,
                _jwtHandler,
                _blobStorageService,
                _configuration,
                _mapper);

            // Act
            var result = await employeeService.UpdateProfileAsync(usernameFromClaim, profileUpdateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Profile updated successfully");

            var updatedEmployee = result.Response as Employee;
            updatedEmployee.Should().NotBeNull();
            updatedEmployee.EmployeeName.Should().Be("Alison Timberlee");
            updatedEmployee.Status.Should().Be("Active");
            updatedEmployee.EmploymentType.Should().Be("Full-Time");
            updatedEmployee.WorkEmail.Should().Be("user1@datafirstservices.com");
        }


        [Fact]
        public async Task EmployeeService_UpdateProfileAsync_ReturnsErrorWhenUserNameNotMatch()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            string usernameFromClaim = "userxyz";

            var profileUpdateDto = new EmployeeProfileUpdateDTO
            {
                EmployeeName = "Alison Timberlee",
                Status = "Active",
                EmploymentType = "Full-Time",
                WorkEmail = "user1@datafirstservices.com",
                UserName = "user1",
                PersonalPhone = "1234567890"
            };

            var user = new Entities.User
            {
                UserName = "user1",
                Email = "user1@datafirstservices.com",
                EmployeeName = "A. Gaud",
            };

            // Mock UserManager behavior
            A.CallTo(() => _userManager.FindByNameAsync(usernameFromClaim))
                .Returns((Entities.User?)null);

            A.CallTo(() => _userManager.UpdateAsync(A<Entities.User>.Ignored))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Mock AutoMapper mapping behavior
            A.CallTo(() => _mapper.Map(A<EmployeeProfileUpdateDTO>.Ignored, A<Employee>.Ignored))
                .Invokes((EmployeeProfileUpdateDTO src, Employee dest) =>
                {
                    dest.EmployeeName = src.EmployeeName;
                    dest.UserName = src.UserName;
                    dest.WorkEmail = src.WorkEmail;
                    dest.Status = src.Status;
                    dest.EmploymentType = src.EmploymentType;
                });

            var employeeService = new EmployeeService(
                _context,
                _userManager,
                _jwtHandler,
                _blobStorageService,
                _configuration,
                _mapper);

            // Act
            var result = await employeeService.UpdateProfileAsync(usernameFromClaim, profileUpdateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("User not found");
            result.Response.Should().BeNull();
        }

        [Fact]
        public async Task EmployeeService_UpdateProfileAsync_ReturnsErrorWhenEmployeeNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            string usernameFromClaim = "userxyz";

            var profileUpdateDto = new EmployeeProfileUpdateDTO
            {
                EmployeeName = "Alison Timberlee",
                Status = "Active",
                EmploymentType = "Full-Time",
                WorkEmail = "user1@datafirstservices.com",
                UserName = "user1",
                PersonalPhone = "1234567890"
            };

            var user = new Entities.User
            {
                UserName = "user1",
                Email = "user1@datafirstservices.com",
                EmployeeName = "A. Gaud",
            };

            // Mock UserManager behavior
            A.CallTo(() => _userManager.FindByNameAsync(A<string>.Ignored))
                .Returns(user);

            A.CallTo(() => _userManager.UpdateAsync(A<Entities.User>.Ignored))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Mock AutoMapper mapping behavior
            A.CallTo(() => _mapper.Map(A<EmployeeProfileUpdateDTO>.Ignored, A<Employee>.Ignored))
                .Invokes((EmployeeProfileUpdateDTO src, Employee dest) =>
                {
                    dest.EmployeeName = src.EmployeeName;
                    dest.UserName = src.UserName;
                    dest.WorkEmail = src.WorkEmail;
                    dest.Status = src.Status;
                    dest.EmploymentType = src.EmploymentType;
                });

            var employeeService = new EmployeeService(
                _context,
                _userManager,
                _jwtHandler,
                _blobStorageService,
                _configuration,
                _mapper);

            // Act
            var result = await employeeService.UpdateProfileAsync(usernameFromClaim, profileUpdateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Employee profile not found");

        }

        [Fact]
        public async Task EmployeeService_UpdateProfileAsync_ReturnsErrorWhenUpdateAsyncFails()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            string usernameFromClaim = "user1";

            var profileUpdateDto = new EmployeeProfileUpdateDTO
            {
                EmployeeName = "Alison Timberlee",
                Status = "Active",
                EmploymentType = "Full-Time",
                WorkEmail = "user1@datafirstservices.com",
                UserName = "user1",
                PersonalPhone = "1234567890"
            };

            var user = new Entities.User
            {
                UserName = "user1",
                Email = "user1@datafirstservices.com",
                EmployeeName = "A. Gaud",
            };

            // Mock UserManager behavior
            A.CallTo(() => _userManager.FindByNameAsync(A<string>.Ignored))
                .Returns(user);

            A.CallTo(() => _userManager.UpdateAsync(A<Entities.User>.Ignored))
                .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "User update failed" })));

            // Mock AutoMapper mapping behavior
            A.CallTo(() => _mapper.Map(A<EmployeeProfileUpdateDTO>.Ignored, A<Employee>.Ignored))
                .Invokes((EmployeeProfileUpdateDTO src, Employee dest) =>
                {
                    dest.EmployeeName = src.EmployeeName;
                    dest.UserName = src.UserName;
                    dest.WorkEmail = src.WorkEmail;
                    dest.Status = src.Status;
                    dest.EmploymentType = src.EmploymentType;
                });

            var employeeService = new EmployeeService(
                _context,
                _userManager,
                _jwtHandler,
                _blobStorageService,
                _configuration,
                _mapper);

            // Act
            var result = await employeeService.UpdateProfileAsync(usernameFromClaim, profileUpdateDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task EmployeeService_UpdateProfileAsync_SuccessfullyReflectsInDatabase()
        {
            // Arrange
            var _context = await GetDatabaseContext();

            string usernameFromClaim = "user1";

            var profileUpdateDto = new EmployeeProfileUpdateDTO
            {
                EmployeeName = "Alison Timberlee",
                Status = "Active",
                EmploymentType = "Full-Time",
                WorkEmail = "user1@datafirstservices.com",
                UserName = "user1",
                PersonalPhone = "1234567890"
            };

            var user = new Entities.User
            {
                UserName = "user1",
                Email = "user1@datafirstservices.com",
                EmployeeName = "A. Gaud",
            };

            // Mock UserManager behavior
            A.CallTo(() => _userManager.FindByNameAsync(A<string>.Ignored))
                .Returns(user);

            A.CallTo(() => _userManager.UpdateAsync(A<Entities.User>.Ignored))
                .Returns(Task.FromResult(IdentityResult.Success));

            // Mock AutoMapper mapping behavior
            A.CallTo(() => _mapper.Map(A<EmployeeProfileUpdateDTO>.Ignored, A<Employee>.Ignored))
                .Invokes((EmployeeProfileUpdateDTO src, Employee dest) =>
                {
                    dest.EmployeeName = src.EmployeeName;
                    dest.UserName = src.UserName;
                    dest.WorkEmail = src.WorkEmail;
                    dest.Status = src.Status;
                    dest.EmploymentType = src.EmploymentType;
                });

            var employeeService = new EmployeeService(
                _context,
                _userManager,
                _jwtHandler,
                _blobStorageService,
                _configuration,
                _mapper);

            // Act
            var result = await employeeService.UpdateProfileAsync(usernameFromClaim, profileUpdateDto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Profile updated successfully");

            var updatedEmployee = result.Response as Employee;
            updatedEmployee.Should().NotBeNull();
            updatedEmployee.EmployeeName.Should().Be("Alison Timberlee");
            updatedEmployee.Status.Should().Be("Active");
            updatedEmployee.EmploymentType.Should().Be("Full-Time");
            updatedEmployee.WorkEmail.Should().Be("user1@datafirstservices.com");


            // Verify changes in the database
            var employeeInDB = _context.Employees.FirstOrDefault(e => e.UserName == "user1");
            employeeInDB.EmployeeName.Should().Be("Alison Timberlee");

        }



        // GetProfileAsync Method
        [Fact]
        public async Task EmployeeService_GetProfileAsync_ReturnsProfileSuccessfully()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";

            var user = new Entities.User
            {
                UserName = "user1",
                Email = "user1@datafirstservices.com",
                EmployeeName = "A. Gaud",
            };



            //A.CallTo(() => _userManager.FindByNameAsync(usernameFromClaim)
            //        .Returns(user);

            A.CallTo(() => _userManager.FindByNameAsync(usernameFromClaim))
                    .Returns(user);

            var profile = new EmployeeProfileDTO
            {
                EmployeeId = 1,
                UserName = "user1",
                IsActive = true,
                EmployeeRole = "Employee",
                EmployeeName = "A. Gaud",
                WorkEmail = "user1@datafirstservices.com"
            };

            A.CallTo(() => _mapper.Map<EmployeeProfileDTO>(A<Employee>.Ignored))
                    .Returns(profile);


            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);

            // Act

            var result = await EmployeeService.GetProfileAsync(usernameFromClaim);

            // Assert

            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            var returnedProfile = result.Response as EmployeeProfileDTO;
            returnedProfile.Should().NotBeNull();
            returnedProfile.EmployeeName.Should().Be("A. Gaud");
            returnedProfile.WorkEmail.Should().Be("user1@datafirstservices.com");


        }




        [Fact]
        public async Task EmployeeService_GetProfileAsync_ReturnsErrorWhenUserNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";

            A.CallTo(() => _userManager.FindByNameAsync(usernameFromClaim))
                    .Returns(Task.FromResult<Entities.User?>(null));


            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);

            // Act

            var result = await EmployeeService.GetProfileAsync(usernameFromClaim);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("User not found");

        }

        [Fact]
        public async Task EmployeeService_GetProfileAsync_ReturnsErrorWhenEmployeeNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";

            var user = new Entities.User
            {
                UserName = "user1",
                Email = "user1@datafirstservices.com",
                EmployeeName = "A. Gaud",
            };


            A.CallTo(() => _userManager.FindByNameAsync(usernameFromClaim))
                            .Returns(Task.FromResult(user));

            _context.Employees.RemoveRange(_context.Employees);
            await _context.SaveChangesAsync();

            var EmployeeService = new EmployeeService(_context, _userManager, _jwtHandler, _blobStorageService, _configuration, _mapper);

            // Act

            var result = await EmployeeService.GetProfileAsync(usernameFromClaim);

            // Assert

            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Employee profile not found");
        }














    }
}







