using FluentAssertions;
using HRManagement.Data;
using HRManagement.Models.Leaves;
using HRManagement.Services.LeaveTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRManagement.Tests.Services
{
    public class LeaveTypeServiceTests
    {
        private AppDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new AppDbContext(options);
        }

        


        [Fact]
        public async Task GetAllLeaveTypesAsync_ReturnsLeaveTypes_WhenPresent()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb1");
            context.LeaveTypes.Add(new LeaveType
            {
                LeaveTypeId = 1,
                LeaveTypeName = "Annual",
                LeaveTypeDescription = "Annual leave",
                DefaultAnnualAllocation = 15
            });
            context.LeaveTypes.Add(new LeaveType
            {
                LeaveTypeId = 2,
                LeaveTypeName = "Sick",
                LeaveTypeDescription = "Sick leave",
                DefaultAnnualAllocation = 10
            });
            await context.SaveChangesAsync();

            var service = new LeaveTypeService(context);

            // Act
            var result = await service.GetAllLeaveTypesAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
            var leaveTypes = (List<LeaveType>)result.Response;
            leaveTypes.Count.Should().Be(2);
            leaveTypes.Should().ContainSingle(x => x.LeaveTypeName == "Annual");
            leaveTypes.Should().ContainSingle(x => x.LeaveTypeName == "Sick");
        }

        [Fact]
        public async Task GetAllLeaveTypesAsync_ReturnsEmptyList_WhenNoLeaveTypes()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb2");
            var service = new LeaveTypeService(context);

            // Act
            var result = await service.GetAllLeaveTypesAsync();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Response.Should().NotBeNull();
            var leaveTypes = (List<LeaveType>)result.Response;
            leaveTypes.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLeaveTypeByIdAsync_ReturnsExistingLeaveType_WhenIdIsValid()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb3");
            context.LeaveTypes.Add(new LeaveType
            {
                LeaveTypeId = 1,
                LeaveTypeName = "Annual",
                LeaveTypeDescription = "Annual leave",
                DefaultAnnualAllocation = 15
            });
            await context.SaveChangesAsync();
            var service = new LeaveTypeService(context);
            // Act
            var result = await service.GetLeaveTypeByIdAsync(1);
            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            ((LeaveType)result.Response).LeaveTypeName.Should().Be("Annual");

        }

        [Fact]
        public async Task GetLeaveTypeByIdAsync_ReturnsLeaveTypeNotFound_WhenIdIsNotValid()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb4");
            context.LeaveTypes.Add(new LeaveType
            {
                LeaveTypeId = 1,
                LeaveTypeName = "Annual",
                LeaveTypeDescription = "Annual leave",
                DefaultAnnualAllocation = 15
            });
            await context.SaveChangesAsync();
            var service = new LeaveTypeService(context);
            // Act
            var result = await service.GetLeaveTypeByIdAsync(999);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Response.Should().BeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task CreateLeaveTypeAsync_CreatesLeaveTypeSuccessfully()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb5");
            var service = new LeaveTypeService(context);
            var dto = new HRManagement.DTOs.Leaves.CreateLeaveTypeDto
            {
                LeaveTypeName = "Maternity",
                LeaveTypeDescription = "Maternity leave",
                DefaultAnnualAllocation = 90
            };
            // Act
            var result = await service.CreateLeaveTypeAsync(dto);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            var createdLeaveType = (LeaveType)result.Response;
            createdLeaveType.LeaveTypeName.Should().Be("Maternity");
            createdLeaveType.LeaveTypeDescription.Should().Be("Maternity leave");
            createdLeaveType.DefaultAnnualAllocation.Should().Be(90);
        }


        [Fact]
        public async Task CreateLeaveTypeAsync_PersistsDataCorrectly()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb8");
            var service = new LeaveTypeService(context);
            var dto = new HRManagement.DTOs.Leaves.CreateLeaveTypeDto
            {
                LeaveTypeName = "Paternity",
                LeaveTypeDescription = "Paternity leave",
                DefaultAnnualAllocation = 10
            };
            // Act
            var result = await service.CreateLeaveTypeAsync(dto);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            var createdLeaveType = (LeaveType)result.Response;
            // Verify it was added to the database
            var leaveTypeInDb = await context.LeaveTypes.FindAsync(createdLeaveType.LeaveTypeId);
            leaveTypeInDb.Should().NotBeNull();
            leaveTypeInDb.LeaveTypeName.Should().Be("Paternity");
            leaveTypeInDb.LeaveTypeDescription.Should().Be("Paternity leave");
            leaveTypeInDb.DefaultAnnualAllocation.Should().Be(10);
        }


        [Fact]
        public async Task UpdateLeaveTypeAsync_UpdatesExistingLeaveTypeSuccessfully()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb6");
            var existingLeaveType = new LeaveType
            {
                LeaveTypeId = 1,
                LeaveTypeName = "Annual",
                LeaveTypeDescription = "Annual leave",
                DefaultAnnualAllocation = 15
            };
            await context.LeaveTypes.AddAsync(existingLeaveType);
            await context.SaveChangesAsync();
            var service = new LeaveTypeService(context);
            var dto = new HRManagement.DTOs.Leaves.LeaveTypeDto
            {
                LeaveTypeId = 1,
                LeaveTypeName = "Updated Annual",
                LeaveTypeDescription = "Updated annual leave description",
                DefaultAnnualAllocation = 20
            };
            // Act
            var result = await service.UpdateLeaveTypeAsync(dto);
            // Assert
            result.IsSuccess.Should().BeTrue();
            result.StatusCode.Should().Be(200);
            var updatedLeaveType = (LeaveType)result.Response;
            updatedLeaveType.LeaveTypeName.Should().Be("Updated Annual");
            updatedLeaveType.LeaveTypeDescription.Should().Be("Updated annual leave description");
            updatedLeaveType.DefaultAnnualAllocation.Should().Be(20);
            // Verify it was updated in the database
            var leaveTypeInDb = await context.LeaveTypes.FindAsync(existingLeaveType.LeaveTypeId);
            leaveTypeInDb.LeaveTypeName.Should().Be("Updated Annual");
        }

        [Fact]
        public async Task UpdateLeaveTypeAsync_ReturnsLeaveTypeNotFound_WhenIdIsNotValid()
        {
            // Arrange
            var context = GetDbContext("LeaveTypesDb7");
            var existingLeaveType = new LeaveType
            {
                LeaveTypeId = 1,
                LeaveTypeName = "Annual",
                LeaveTypeDescription = "Annual leave",
                DefaultAnnualAllocation = 15
            };
            await context.LeaveTypes.AddAsync(existingLeaveType);
            await context.SaveChangesAsync();
            var service = new LeaveTypeService(context);
            var dto = new HRManagement.DTOs.Leaves.LeaveTypeDto
            {
                LeaveTypeId = 999, // Non-existent ID
                LeaveTypeName = "Non-existent",
                LeaveTypeDescription = "This leave type does not exist",
                DefaultAnnualAllocation = 5
            };
            // Act
            var result = await service.UpdateLeaveTypeAsync(dto);
            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Response.Should().BeNull();
            result.StatusCode.Should().Be(404);
        }


    }
}
