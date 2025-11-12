using DocumentFormat.OpenXml.Wordprocessing;
using FakeItEasy;
using FluentAssertions;
using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Enums;
using HRManagement.Helpers;
using HRManagement.Models;
using HRManagement.Models.Leaves;
using HRManagement.Services.BlobStorage;
using HRManagement.Services.Emails;
using HRManagement.Services.LeaveRequests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagement.Tests.Services
{
    public class LeaveRequestServiceTests
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IEmailService _emailService;
        private readonly ILeaveRequestHelper _leaveRequestHelper;   // Transient
        private readonly IConfiguration _configuration;

        public LeaveRequestServiceTests()
        {
            _blobStorageService = A.Fake<IBlobStorageService>();
            _emailService = A.Fake<IEmailService>();
            _leaveRequestHelper = A.Fake<ILeaveRequestHelper>();
            _configuration = A.Fake<IConfiguration>();
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
        public async Task LeaveRequestService_CreateLeaveRequestAsync_CreatesLeaveRequestSuccessfully()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            //A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, -1))
            //                .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            result.Message.Should().Be("Leave request submitted.");



        }



        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_ReturnsErrorWhenEmployeeNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "userx";
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(
                            A<int>.Ignored,
                            A<int>.Ignored,
                            A<DateOnly>.Ignored,
                            A<DateOnly>.Ignored,
                            A<bool>.Ignored,
                            A<bool>.Ignored,
                            A<int>.Ignored))
                           .Returns(Task.FromResult(balanceCheckResponse));



            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            //result.IsSuccess.Should().BeTrue();
            //result.StatusCode.Should().Be(201);
            //result.Message.Should().Be("Leave request submitted.");
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Employee not found for the given username for given token.");

        }


        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_ReturnsErrrorWhenStartDateIsGreaterThanStartDate()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));

            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            //result.IsSuccess.Should().BeTrue();
            //result.StatusCode.Should().Be(201);
            //result.Message.Should().Be("Leave request submitted.");
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("End date can't be before start date.");




        }

        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_ReturnsErrorWhenYearNotSameOfStartDateAndEndDate()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(400)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));

            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("Leave requests spanning multiple years are not allowed. Please create separate requests for each year.");



        }




        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_CreatesLeaveRequestIfAnyPendingRequestOverlaps()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var leaveRequest1 = new LeaveRequest
            {
                EmployeeId = EmployeeId,
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Reason = "Vacation Leave",
                Status = Enums.LeaveRequestStatus.Pending,
                ManagerRemarks = null,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };
            await _context.LeaveRequests.AddAsync(leaveRequest1);
            await _context.SaveChangesAsync();
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));

            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("There is already an overlapping leave request.");



        }

        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_CreatesLeaveRequestIfLeaveDateInBetweenOtherRequests()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var leaveRequest1 = new LeaveRequest
            {
                EmployeeId = EmployeeId,
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Reason = "Vacation Leave",
                Status = Enums.LeaveRequestStatus.Pending,
                ManagerRemarks = null,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };
            await _context.LeaveRequests.AddAsync(leaveRequest1);
            await _context.SaveChangesAsync();
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(6)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));

            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("There is already an overlapping leave request.");



        }

        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_CreatesLeaveRequestIfAnyApprovedRequestOverlaps()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var leaveRequest1 = new LeaveRequest
            {
                EmployeeId = EmployeeId,
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Reason = "Vacation Leave",
                Status = Enums.LeaveRequestStatus.Pending,
                ManagerRemarks = null,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };
            await _context.LeaveRequests.AddAsync(leaveRequest1);
            await _context.SaveChangesAsync();
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));

            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("There is already an overlapping leave request.");



        }



        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_ReturnsErrorWhenLeaveBalanceNotSufficient()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);

            //decimal requestedLeaveDays = 3m;



            

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("Insufficient leave balance for this request.");
        }


        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_ReturnsErrorIfFileNotSubmittedForSickLeaveGreaterThanTwoDays()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 2,
                StartDate = new DateOnly(2025,11,10),
                EndDate = new DateOnly(2025, 11, 14),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            //A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, -1))
            //                .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("For Sick Leave more than 2 days, uploading a medical certificate is mandatory.");



        }


        [Fact]
        public async Task LeaveRequestService_CreateLeaveRequestAsync_AutoApprovesWhenSickLeaveIsLessThanTwoDays()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();
            string usernameFromClaim = "user1";
            var dto = new CreateLeaveRequestDto
            {
                LeaveTypeId = 2,
                StartDate = new DateOnly(2025, 11, 10),
                EndDate = new DateOnly(2025, 11, 10),
                Reason = "Vacation Leave",
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false,
                Files = null // No files for this test case
            };

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            //decimal requestedLeaveDays = 3m;



            //A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, -1))
            //                .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            result.Message.Should().Be("Leave request submitted.");
            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .MustHaveHappenedOnceExactly();
            
        }










        // Get Leave Requests Endpoint
        [Fact]
        public async Task LeaveRequestService_GetLeaveRequestsForEmployeeAsync_ReturnsLeaveRequestsSuccessfully()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";
            var filters = new GetLeaveRequestsForEmployeeFilterDto
            {
                Status = LeaveRequestStatus.Pending,
                StartDate = new DateOnly(2025,1,1),
                EndDate = new DateOnly(2025, 12, 10)
            };

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 11, 10),
                EndDate = new DateOnly(2025, 11, 10),
                Reason = "Vacation Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            
            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Leave requests fetched successfully.");


            List<GetLeaveRequestsForEmployeeDto> Response = (List<GetLeaveRequestsForEmployeeDto>)result.Response;

            Response.Should().HaveCount(1); 
            Response[0].EmployeeId.Should().Be(EmployeeId);
            Response[0].EmployeeName.Should().Be("A. Gaud");
        }







        [Fact]
        public async Task LeaveRequestService_GetLeaveRequestsForEmployeeAsync_ReturnsErrorWhenEmployeeNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user112";
            var filters = new GetLeaveRequestsForEmployeeFilterDto
            {
                Status = LeaveRequestStatus.Pending,
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 12, 10)
            };

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 11, 10),
                EndDate = new DateOnly(2025, 11, 10),
                Reason = "Vacation Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            



            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Employee not found");

        }







        [Fact]
        public async Task LeaveRequestService_GetLeaveRequestsForEmployeeAsync_ReturnsOnlyApprovedLeaveRequests()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";
            var filters = new GetLeaveRequestsForEmployeeFilterDto
            {
                Status = LeaveRequestStatus.Approved,
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 12, 10)
            };

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj1 = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 10, 10),
                EndDate = new DateOnly(2025, 10, 10),
                Reason = "Vacation Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj2 = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 11, 11),
                EndDate = new DateOnly(2025, 11, 11),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };



            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj1);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj2);
            await _context.SaveChangesAsync();

            



            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Leave requests fetched successfully.");


            List<GetLeaveRequestsForEmployeeDto> Response = (List<GetLeaveRequestsForEmployeeDto>)result.Response;

            Response.Should().HaveCount(1);
            Response[0].EmployeeId.Should().Be(EmployeeId);
            Response[0].EmployeeName.Should().Be("A. Gaud");
            Response[0].LeaveRequestId.Should().Be(2);
        }




        [Fact]
        public async Task LeaveRequestService_GetLeaveRequestsForEmployeeAsync_ReturnsOnlyApprovedAndAfterStartDate()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";
            var filters = new GetLeaveRequestsForEmployeeFilterDto
            {
                Status = LeaveRequestStatus.Approved,
                StartDate = new DateOnly(2025, 3, 1),
                //EndDate = new DateOnly(2025, 12, 10)
            };

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj1 = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 1, 1),
                Reason = "Vacation Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj2 = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 2),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            var leaveRequestSeedObj3 = new LeaveRequest
            {
                LeaveTypeId = 3,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 3, 3),
                EndDate = new DateOnly(2025, 3, 3),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };



            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj1);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj2);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj3);
            await _context.SaveChangesAsync();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Leave requests fetched successfully.");


            List<GetLeaveRequestsForEmployeeDto> Response = (List<GetLeaveRequestsForEmployeeDto>)result.Response;

            Response.Should().HaveCount(1);
            Response[0].EmployeeId.Should().Be(EmployeeId);
            Response[0].EmployeeName.Should().Be("A. Gaud");
            Response[0].LeaveRequestId.Should().Be(3);
        }




        [Fact]
        public async Task LeaveRequestService_GetLeaveRequestsForEmployeeAsync_ReturnsOnlyApprovedAndAfterStartDateAndBeforeEndDate()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";
            var filters = new GetLeaveRequestsForEmployeeFilterDto
            {
                Status = LeaveRequestStatus.Approved,
                StartDate = new DateOnly(2025, 3, 1),
                EndDate = new DateOnly(2025, 4, 4)
            };

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj1 = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 1, 1),
                Reason = "Vacation Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj2 = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 2),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            var leaveRequestSeedObj3 = new LeaveRequest
            {
                LeaveTypeId = 3,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 3, 3),
                EndDate = new DateOnly(2025, 3, 3),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj4 = new LeaveRequest
            {
                LeaveTypeId = 4,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 4, 1),
                EndDate = new DateOnly(2025, 4, 1),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj5 = new LeaveRequest
            {
                LeaveTypeId = 4,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 4, 2),
                EndDate = new DateOnly(2025, 4, 4),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };



            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj1);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj2);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj3);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj4);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj5);
            await _context.SaveChangesAsync();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Leave requests fetched successfully.");


            List<GetLeaveRequestsForEmployeeDto> Response = (List<GetLeaveRequestsForEmployeeDto>)result.Response;

            Response.Should().HaveCount(3);
            Response[0].EmployeeId.Should().Be(EmployeeId);
            Response[1].EmployeeId.Should().Be(Response[2].EmployeeId);
            Response[0].EmployeeName.Should().Be("A. Gaud");
            Response[0].LeaveRequestId.Should().Be(5);
            Response[1].LeaveRequestId.Should().Be(4);
            Response[2].LeaveRequestId.Should().Be(3);
        }






        [Fact]
        public async Task LeaveRequestService_GetLeaveRequestsForEmployeeAsync_ReturnsErrorWhenNoRecordsFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";
            var filters = new GetLeaveRequestsForEmployeeFilterDto
            {
                Status = LeaveRequestStatus.Rejected,
                StartDate = new DateOnly(2025, 3, 1),
                EndDate = new DateOnly(2025, 4, 4)
            };

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj1 = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 1, 1),
                Reason = "Vacation Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj2 = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 2),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            var leaveRequestSeedObj3 = new LeaveRequest
            {
                LeaveTypeId = 3,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 3, 3),
                EndDate = new DateOnly(2025, 3, 3),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj4 = new LeaveRequest
            {
                LeaveTypeId = 4,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 4, 1),
                EndDate = new DateOnly(2025, 4, 1),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj5 = new LeaveRequest
            {
                LeaveTypeId = 4,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 4, 2),
                EndDate = new DateOnly(2025, 4, 4),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };



            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj1);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj2);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj3);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj4);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj5);
            await _context.SaveChangesAsync();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("No leave requests found.");

        }






        [Fact]
        public async Task LeaveRequestService_GetLeaveRequestsForEmployeeAsync_PopulatesLeaveTypeNameCorrectly()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            string usernameFromClaim = "user1";
            var filters = new GetLeaveRequestsForEmployeeFilterDto
            {
                Status = LeaveRequestStatus.Pending,
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 4, 4)
            };

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj1 = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 1, 1),
                EndDate = new DateOnly(2025, 1, 1),
                Reason = "Vacation Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj2 = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 2),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            var leaveRequestSeedObj3 = new LeaveRequest
            {
                LeaveTypeId = 3,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 3, 3),
                EndDate = new DateOnly(2025, 3, 3),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj4 = new LeaveRequest
            {
                LeaveTypeId = 4,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 4, 1),
                EndDate = new DateOnly(2025, 4, 1),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };


            var leaveRequestSeedObj5 = new LeaveRequest
            {
                LeaveTypeId = 4,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 4, 4),
                EndDate = new DateOnly(2025, 4, 5),
                Reason = "Leave due to Fever",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 1,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };



            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj1);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj2);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj3);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj4);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj5);
            await _context.SaveChangesAsync();





            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Leave requests fetched successfully.");


            List<GetLeaveRequestsForEmployeeDto> Response = (List<GetLeaveRequestsForEmployeeDto>)result.Response;

            Response.Should().HaveCount(1);
            Response[0].EmployeeId.Should().Be(EmployeeId);
            Response[0].EmployeeName.Should().Be("A. Gaud");
            Response[0].LeaveRequestId.Should().Be(1);
            Response[0].LeaveTypeName.Should().Be("Annual Leave");
        }












        // Update Leave Requests Method - For Employee
        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_UpdatesLeaveRequestSuccessfully()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 12),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";
            

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Request updated.");


            LeaveRequest Response = (LeaveRequest)result.Response;

            Response.LeaveRequestId.Should().Be(requestId);
            Response.LeaveTypeId.Should().Be(dto.LeaveTypeId);
            Response.StartDate.Should().Be(dto.StartDate);
            Response.EndDate.Should().Be(dto.EndDate);
            Response.Reason.Should().Be(dto.Reason);
            Response.IsStartDateHalfDay.Should().Be(dto.IsStartDateHalfDay);
            Response.IsEndDateHalfDay.Should().Be(dto.IsEndDateHalfDay);
            Response.LeaveDaysUsed.Should().Be(2.5m);
        }



        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorWhenEmployeeNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 12),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1112";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
        }






        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorWhenLeaveRequestNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 999;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 999,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 12),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Request not found.");

        }




        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorIfEmployeeTriesToEditOtherEmployeesLeaveRequest()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 12),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = 999,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(403);
            result.Message.Should().Be("You can only update your own requests.");

        }







        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorIfUserTriesToUpdateAlreadyApprovedLeaveRequest()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 12),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("Can only modify pending requests.");

        }





        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorIfUserTriesToUpdateIntoMultipleSpanningYear()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2026, 12, 12),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("Leave requests spanning multiple years are not allowed. Please create separate requests for each year.");

        }




        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorIfStartDateIsAfterEndDate()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 12, 12),
                EndDate = new DateOnly(2025, 12, 10),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("End date can't be before start date.");

        }





        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorIfOverlapExists()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 5),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj1 = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            var leaveRequestSeedObj2 = new LeaveRequest
            {
                LeaveTypeId = 1,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Approved,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj1);
            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj2);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("There is already an overlapping leave request.");

        }





        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorWhenInsufficientBalance()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 1,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 12),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse ();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("Insufficient leave balance for this request.");


        }

        
        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_ReturnsErrorIfSickLeaveWithMoreThanTwoDaysAndFilNotUploaded()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 2,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 20),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));




            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(400);
            result.Message.Should().Be("For Sick Leave more than 2 days, uploading a medical certificate is mandatory.");
        }



        [Fact]
        public async Task LeaveRequestService_UpdateLeaveRequestAsync_AutoApprovesRequestIfSickLeaveForUptoTwoDays()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            var dto = new UpdateLeaveRequestDto
            {
                LeaveTypeId = 2 ,
                StartDate = new DateOnly(2025, 12, 10),
                EndDate = new DateOnly(2025, 12, 11),
                Reason = "Updated Reason of Sick Leave",
                IsStartDateHalfDay = true,
                IsEndDateHalfDay = false
            };
            string usernameFromClaim = "user1";


            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var balanceCheckResponse = new ApiResponse(true, "Leave balance is sufficient.", 200, null);

            A.CallTo(() => _leaveRequestHelper.CheckLeaveBalanceCoreAsync(EmployeeId, dto.LeaveTypeId, dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay, A<int?>.Ignored))
                            .Returns(Task.FromResult(balanceCheckResponse));


            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .DoesNothing();

            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Request updated.");


            LeaveRequest Response = (LeaveRequest)result.Response;

            Response.Status.Should().Be(LeaveRequestStatus.Approved);
            A.CallTo(() => _emailService.SendEmail(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored))
                            .MustHaveHappenedOnceExactly();
        }












        // Cancel Leave Requests Method - For Employee

        [Fact]
        public async Task LeaveRequestService_CancelLeaveRequestAsync_CancelsSuccessfully()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;  
            string usernameFromClaim = "user1";

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CancelLeaveRequestAsync(requestId, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            result.Message.Should().Be("Leave request withdrawn successfully.");


            LeaveRequest Response = (LeaveRequest)result.Response;

            Response.Status.Should().Be(LeaveRequestStatus.Cancelled);
            

            var req = await _context.LeaveRequests.FindAsync(requestId);
            req.Status.Should().Be(LeaveRequestStatus.Cancelled);

        }



        [Fact]
        public async Task LeaveRequestService_CancelLeaveRequestAsync_ReturnsErrorWhenEmployeeNotFound()
        {
            // Arrange
            var _context = await GetDatabaseContext();
            int requestId = 1;
            string usernameFromClaim = "user231";

            var EmployeeId = await _context.Employees.Where(e => e.UserName == "user1")
                .Select(e => e.EmployeeId)
                .FirstOrDefaultAsync();

            var leaveRequestSeedObj = new LeaveRequest
            {
                LeaveTypeId = 2,
                EmployeeId = EmployeeId,
                StartDate = new DateOnly(2025, 2, 2),
                EndDate = new DateOnly(2025, 2, 4),
                Reason = "Sick Leave",
                Status = LeaveRequestStatus.Pending,
                RequestedOn = DateTime.UtcNow,
                LeaveDaysUsed = 3m,
                IsStartDateHalfDay = false,
                IsEndDateHalfDay = false
            };

            await _context.LeaveRequests.AddAsync(leaveRequestSeedObj);
            await _context.SaveChangesAsync();

            var leaveRequestService = new LeaveRequestService(
                _context,
                _configuration,
                _blobStorageService,
                _emailService,
                _leaveRequestHelper
                );


            // Act

            var result = await leaveRequestService.CancelLeaveRequestAsync(requestId, usernameFromClaim);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.StatusCode.Should().Be(404);
            result.Message.Should().Be("Employee not found for the given username for given token.");
            result.Response.Should().Be(null);
        }
















    }
}
