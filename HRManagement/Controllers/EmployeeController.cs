using HRManagement.DTOs;
using HRManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Controllers
{
    [Authorize(Roles = "Admin")]
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
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var Response = await _employeeService.GetAllEmployees();
            return Ok(Response); 
        }

        // Get https://localhost:7150/api/employee/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var Response = await _employeeService.GetEmployeeById(id);
            return Ok(Response);
        }

        // POST  https://localhost:7150/api/employee/
        [HttpPost]
        public async Task<ActionResult> CreateEmployee(EmployeeCreateDTO employeeDto)
        {
            var Response = await _employeeService.CreateEmployee(employeeDto);
            return Ok(Response);
        }

        // PUT  https://localhost:7150/api/employee/
        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(EmployeeUpdateDTO updated)
        {
            var Response = await _employeeService.UpdateEmployee(updated);
            return Ok(Response);
        }

        // DELETE https://localhost:7150/api/employee/7
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var Response = await _employeeService.DeleteEmployee(id);
            return Ok(Response);
        }
    }

}
