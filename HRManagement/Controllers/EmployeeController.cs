using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.Models;
using HRManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var Response = await _employeeService.GetAllEmployees();
            return Ok(Response); 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var Response = await _employeeService.GetEmployeeById(id);
            return Ok(Response);
        }

        [HttpPost]
        public async Task<ActionResult> CreateEmployee(EmployeeCreateDTO employeeDto)
        {
            var Response = await _employeeService.CreateEmployee(employeeDto);
            return Ok(Response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateEmployee(EmployeeUpdateDTO updated)
        {
            var Response = await _employeeService.UpdateEmployee(updated);
            return Ok(Response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var Response = await _employeeService.DeleteEmployee(id);
            return Ok(Response);
        }
    }

}
