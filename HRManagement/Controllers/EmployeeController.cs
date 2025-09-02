using HRManagement.DTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Services.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }



        // Get https://localhost:7150/api/employee/
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var Response = await _employeeService.GetAllEmployees();
            return Ok(Response); 
        }

        // Get https://localhost:7150/api/employee/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var Response = await _employeeService.GetEmployeeById(id);
            return Ok(Response);
        }

        // POST  https://localhost:7150/api/employee/
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateEmployee(EmployeeCreateDTO employeeDto)
        {
            var Response = await _employeeService.CreateEmployee(employeeDto);
            return Ok(Response);
        }


        // PUT  https://localhost:7150/api/employee/
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(EmployeeUpdateDTO updated)
        {
            var Response = await _employeeService.UpdateEmployee(updated);
            return Ok(Response);
        }

        // DELETE https://localhost:7150/api/employee/7
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var Response = await _employeeService.DeleteEmployee(id);
            return Ok(Response);
        }


        // For user itself
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(EmployeeProfileUpdateDTO profileUpdateDto)
        {
            // Get current logged-in user's username from JWT claims
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usernameFromClaim))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));

            var response = await _employeeService.UpdateProfileAsync(usernameFromClaim, profileUpdateDto);
            return StatusCode(response.StatusCode, response);
        }

        // For user itself
        [Authorize]
        [HttpPost("upload-profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usernameFromClaim))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));

            var response = await _employeeService.UploadProfilePictureAsync(usernameFromClaim, file);
            return StatusCode(response.StatusCode, response);
        }

        // For user itself
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            string username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));

            var response = await _employeeService.GetProfileAsync(username);
            return StatusCode(response.StatusCode, response);
        }


    }

}
