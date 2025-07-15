using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Services
{
    public class EmployeeService : IEmployeeService
    {

        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        // Implement the methods defined in the IEmployeeService interface here
        public async Task<ApiResponse> GetAllEmployees()
        {
            var employees = await _context.Employees.ToListAsync();

            var response = new ApiResponse
            {
                IsSuccess = true,
                Message = "Employee list retrieved successfully",
                StatusCode = 200,
                Response = employees
            };

            return response;
        }
        public async Task<ApiResponse> GetEmployeeById(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == id);
            if (employee == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Employee not found",
                    StatusCode = 404,
                    Response = null
                };

            }

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Employee retrieved successfully",
                StatusCode = 200,
                Response = employee
            };

        }
        public async Task<ApiResponse> CreateEmployee(EmployeeCreateDTO employeeDto)
        {
            var employee = new Employee
            {
                Email = employeeDto.Email,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Username = employeeDto.Username,
                Phone = employeeDto.Phone,
                IsActive = employeeDto.IsActive,
                CreatedBy = employeeDto.CreatedBy,
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = employeeDto.CreatedBy,
                ModifiedDate = DateTime.UtcNow
            };



            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Employee created successfully",
                StatusCode = 201,
                Response = employee
            };
        }
        public async Task<ApiResponse> UpdateEmployee(EmployeeUpdateDTO updated)
        {
            var employee = await _context.Employees.FindAsync(updated.EmployeeId);
            if (employee == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Invalid employee details",
                    StatusCode = 404,
                    Response = null
                };

            }

            employee.FirstName = updated.FirstName;
            employee.LastName = updated.LastName;
            employee.Email = updated.Email;
            employee.Phone = updated.Phone;
            employee.Username = updated.Username;
            employee.IsActive = updated.IsActive;
            employee.ModifiedBy = updated.ModifiedBy;
            employee.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Employee updated successfully",
                StatusCode = 200,
                Response = employee
            };
        }
        public async Task<ApiResponse> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Employee not found",
                    StatusCode = 404,
                    Response = null
                };
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Employee deleted successfully",
                StatusCode = 200,
                Response = employee
            };
        }
    }
}
