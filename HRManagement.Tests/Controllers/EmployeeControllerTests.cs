using FakeItEasy;
using FluentAssertions;
using HRManagement.Controllers;
using HRManagement.DTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Services.Employees;
using HRManagement.Services.EmployeesExcel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HRManagement.Tests.Controllers
{
    //public class EmployeeControllerTests
    //{
    //}

    public class EmployeeControllerTests
    {
        private readonly IEmployeeService _employeeService = A.Fake<IEmployeeService>();
        private readonly IEmployeeExcel _employeeExcel = A.Fake<IEmployeeExcel>();

        private EmployeeController CreateControllerWithUserClaims(string? username)
        {
            var controller = new EmployeeController(_employeeService, _employeeExcel);

            var user = new ClaimsPrincipal(new ClaimsIdentity(username != null
                ? new[] { new Claim(ClaimTypes.Name, username) }
                : System.Array.Empty<Claim>()));

            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext()
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() { User = user }
            };

            return controller;
        }

        [Fact]
        public async Task UpdateProfile_ReturnsUnauthorized_WhenUsernameClaimMissing()
        {
            var controller = CreateControllerWithUserClaims(null);
            var dto = new EmployeeProfileUpdateDTO();

            var result = await controller.UpdateProfile(dto);

            result.Should().BeOfType<UnauthorizedObjectResult>();
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.StatusCode.Should().Be(401);



        }

        [Fact]
        public async Task UpdateProfile_CallsServiceAndReturnsStatusCode_WhenUsernameClaimPresent()
        {
            var username = "testuser";
            var dto = new EmployeeProfileUpdateDTO();

            var expectedApiResponse = new ApiResponse(true, "Updated", 200, null);

            A.CallTo(() => _employeeService.UpdateProfileAsync(username, dto))
                .Returns(Task.FromResult(expectedApiResponse));

            var controller = CreateControllerWithUserClaims(username);

            var result = await controller.UpdateProfile(dto);

            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(expectedApiResponse.StatusCode);
            objectResult.Value.Should().BeEquivalentTo(expectedApiResponse);

            A.CallTo(() => _employeeService.UpdateProfileAsync(username, dto)).MustHaveHappenedOnceExactly();
        }
    }
}
