using AutoMapper;
using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Entities;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRManagement.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {

        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly JwtHandler _jwtHandler;
        private readonly BlobStorageService _blobStorageService;
        private readonly string _containerNameForProfilePictures;
        private readonly IMapper _mapper;

        public EmployeeService(AppDbContext context, UserManager<User> userManager, JwtHandler jwtHandler, BlobStorageService blobStorageService, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _blobStorageService = blobStorageService;
            _containerNameForProfilePictures = configuration["AzureBlobStorage:ProfilePictureContainerName"];
            _mapper = mapper;
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

            var user = new User
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                UserName = employeeDto.UserName,
                PhoneNumber = employeeDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, employeeDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return new ApiResponse(false, "User registration failed: " + string.Join(", ", errors), 400, errors);

            }

            await _userManager.AddToRoleAsync(user, employeeDto.EmployeeRole);

            //var employee = new Employee
            //{
            //    Email = employeeDto.Email,
            //    FirstName = employeeDto.FirstName,
            //    LastName = employeeDto.LastName,
            //    UserName = employeeDto.UserName,
            //    Phone = employeeDto.PhoneNumber,
            //    IsActive = employeeDto.IsActive,
            //    CreatedBy = employeeDto.CreatedBy,
            //    CreatedDate = DateTime.UtcNow,
            //    ModifiedBy = employeeDto.CreatedBy,
            //    ModifiedDate = DateTime.UtcNow,
            //    EmployeeRole = employeeDto.EmployeeRole
            //};

            // Using Automapper
            var employee = _mapper.Map<Employee>(employeeDto);

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
            // Find employee in HRManagement.Employees table
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

            // Also find corresponding Identity User
            var user = await _userManager.FindByEmailAsync(employee.Email);

            if (user == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Employee's linked user account not found",
                    StatusCode = 404,
                    Response = null
                };
            }

            //// Update Employee table
            //employee.FirstName = updated.FirstName;
            //employee.LastName = updated.LastName;
            //employee.Email = updated.Email;
            //employee.Phone = updated.Phone;
            //employee.UserName = updated.UserName;
            //employee.IsActive = updated.IsActive;
            //employee.ModifiedBy = updated.ModifiedBy;
            //employee.ModifiedDate = DateTime.UtcNow;

            //// Update Users table
            //user.FirstName = updated.FirstName;
            //user.LastName = updated.LastName;
            //user.Email = updated.Email;
            //user.UserName = updated.UserName;
            //user.PhoneNumber = updated.Phone;

            Console.WriteLine($"\n\n\n\n{employee.EmployeeFullName}");

            _mapper.Map(updated, employee);
            employee.ModifiedDate = DateTime.UtcNow; // Set fields not present in DTO manually

            Console.WriteLine($"\n\n\n\n{employee.EmployeeFullName}");


            // Save changes to both tables

            var result = await _userManager.UpdateAsync(user);

            Console.WriteLine($"\n\n\n\n{employee.EmployeeFullName}");
            if (!result.Succeeded)
            {
                // Collect the identity errors
                var errors = result.Errors.Select(e => e.Description).ToList();

                // Return failure response
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "User update failed: " + string.Join(", ", errors),
                    StatusCode = 400,
                    Response = errors
                };
            }


            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Employee and linked user updated successfully",
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

            var user = await _userManager.FindByEmailAsync(employee.Email);

            if (user != null)
            {
                var userDeleteResult = await _userManager.DeleteAsync(user);
                if (!userDeleteResult.Succeeded)
                {
                    var errors = userDeleteResult.Errors.Select(e => e.Description).ToList();
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Failed to delete linked user account: " + string.Join(", ", errors),
                        StatusCode = 400,
                        Response = errors
                    };
                }
            }


            // Delete profile picture blob if exists
            if (!string.IsNullOrEmpty(employee.ProfilePictureFileName))
            {
                try
                {
                    await _blobStorageService.DeleteFileAsync(employee.ProfilePictureFileName, _containerNameForProfilePictures);
                }
                catch (Exception ex)
                {
                    // Log exception, continue without blocking upload
                    // e.g., _logger.LogWarning($"Failed to delete old profile picture blob: {ex.Message}");
                    Console.WriteLine(ex.Message);
                }
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

        public async Task<ApiResponse> UpdateProfileAsync(string usernameFromClaim, EmployeeProfileUpdateDTO profileUpdateDto)
        {
            // Find user by username
            var user = await _userManager.FindByNameAsync(usernameFromClaim);
            if (user == null)
                return new ApiResponse(false, "User not found", 404, null);

            // Find related employee record by email or username
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserName == usernameFromClaim);
            if (employee == null)
                return new ApiResponse(false, "Employee profile not found", 404, null);

            // Update UserManager user fields
            user.FirstName = profileUpdateDto.FirstName;
            user.LastName = profileUpdateDto.LastName;
            user.PhoneNumber = profileUpdateDto.Phone;
            user.UserName = profileUpdateDto.UserName;
            user.Email = profileUpdateDto.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return new ApiResponse(false, "User update failed: " + string.Join(", ", errors), 400, errors);
            }

            //// Update Employee table fields
            //employee.FirstName = profileUpdateDto.FirstName;
            //employee.LastName = profileUpdateDto.LastName;
            //employee.Phone = profileUpdateDto.Phone;
            //employee.UserName = profileUpdateDto.UserName;
            //employee.Email = profileUpdateDto.Email;
            //employee.ModifiedBy = $"{user.FirstName} {user.LastName}";
            //employee.ModifiedDate = DateTime.UtcNow;

            _mapper.Map(profileUpdateDto, employee);
            employee.ModifiedDate = DateTime.UtcNow; // Set fields not present in DTO manually
            employee.ModifiedBy = $"{user.FirstName} {user.LastName}";

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Profile updated successfully", 200, employee);
        }


        public async Task<ApiResponse> UploadProfilePictureAsync(string usernameFromClaim, IFormFile file)
        {
            if (file == null || file.Length == 0)   
                return new ApiResponse(false, "No file provided", 400, null);


            // Validate file extension and content type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            Console.WriteLine($"\n\n\n\n\nThe file extension is: {fileExtension}");
            if (!allowedExtensions.Contains(fileExtension))
            {
                return new ApiResponse(false, "Only JPG, JPEG and PNG image formats are allowed", 400, null);
            }

            Console.WriteLine($"\n\n\n\n\nThe file content type in lowercase is: {file.ContentType.ToLower()}");
            if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
            {
                return new ApiResponse(false, "Only JPG and PNG image formats are allowed", 400, null);
            }




            var user = await _userManager.FindByNameAsync(usernameFromClaim);
            if (user == null)
                return new ApiResponse(false, "User not found", 404, null);

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserName == usernameFromClaim);
            if (employee == null)
                return new ApiResponse(false, "Employee profile not found", 404, null);

            // Delete old profile picture blob if exists
            if (!string.IsNullOrEmpty(employee.ProfilePictureFileName))
            {
                try
                {
                    await _blobStorageService.DeleteFileAsync(employee.ProfilePictureFileName, _containerNameForProfilePictures);
                }
                catch (Exception ex)
                {
                    // Log exception, continue without blocking upload
                    // e.g., _logger.LogWarning($"Failed to delete old profile picture blob: {ex.Message}");
                    Console.WriteLine(ex.Message);
                }
            }

            var uniqueFileName = $"{usernameFromClaim}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string blobName = await _blobStorageService.UploadFileAsync(file, uniqueFileName, _containerNameForProfilePictures);

            employee.ProfilePictureFileName = blobName;
            employee.ModifiedBy = usernameFromClaim;
            employee.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Profile picture uploaded successfully", 200, new { PictureBlobName = blobName });
        }


        public async Task<ApiResponse> GetProfileAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return new ApiResponse(false, "User not found", 404, null);

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserName == username || e.Email == user.Email);

            if (employee == null)
                return new ApiResponse(false, "Employee profile not found", 404, null);

            string? profilePicUrl = null;
            if (!string.IsNullOrEmpty(employee.ProfilePictureFileName))
            {
                // Generate secure SAS URL from blob name
                profilePicUrl = _blobStorageService.GetTemporaryBlobUrl(employee.ProfilePictureFileName, _containerNameForProfilePictures);
            }

            //var profile = new
            //{
            //    employee.EmployeeId,
            //    employee.FirstName,
            //    employee.LastName,
            //    employee.Email,
            //    employee.UserName,
            //    employee.Phone,
            //    employee.IsActive,
            //    employee.EmployeeRole,
            //    employee.ProfilePictureFileName,
            //    ProfilePictureUrl = profilePicUrl,
            //    employee.CreatedDate,
            //    employee.ModifiedDate
            //};

            var profile = _mapper.Map<EmployeeProfileDTO>(employee);
            profile.ProfilePictureUrl = profilePicUrl;

            return new ApiResponse(true, "Profile retrieved successfully", 200, profile);
        }



    }
}
