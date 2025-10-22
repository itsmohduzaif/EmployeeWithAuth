using HRManagement.DTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Services.Employees;
using HRManagement.Services.EmployeesExcel;
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
        private readonly IEmployeeExcel _employeeExcel; 

        public EmployeeController(IEmployeeService employeeService, IEmployeeExcel employeeExcel)
        {
            _employeeService = employeeService;
            _employeeExcel = employeeExcel;
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

        // POST  https://localhost:7150/api/employee/signup
        [HttpPost("signup")]
        public async Task<ActionResult> SignUpAsAnEmployee(SignUpAsAnEmployeeDTO employeeDto)
        {
            var Response = await _employeeService.SignUpAsAnEmployee(employeeDto);
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

        // Endpoint to export employees to Excel
        [Authorize(Roles = "Admin")]
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportEmployeesExcel()
        {
            var stream = await _employeeExcel.ExportEmployeesToExcel();

            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"Employees_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(stream, contentType, fileName);


            //return Ok("Excel file exported successfully.");
        }



        // Commented out endpoints for file path based import/export - as we dont have requirement for them.

        //// Endpoint to export employees to Excel
        ////[Authorize(Roles = "Admin")]
        //[HttpGet("export-excel-to-file-path")]
        //public async Task<IActionResult> ExportEmployeesExcelToFilePath()
        //{
        //    await _employeeExcel.ExportEmployeesExcelToFilePath(@"C:\Users\user\Downloads\test7.xlsx");
        //    return Ok("Excel file exported successfully.");
        //}

        //// Endpoint to import employees from Excel
        ////[Authorize(Roles = "Admin")]
        //[HttpPost("import-excel-from-file-path")]
        //public async Task<IActionResult> ImportEmployeesExcel()
        //{
        //    await _employeeExcel.ReadEmployeesFromExcelFromPath(@"C:\Users\user\Downloads\test7.xlsx");
        //    return Ok("Excel file imported successfully.");
        //}

    }

}
