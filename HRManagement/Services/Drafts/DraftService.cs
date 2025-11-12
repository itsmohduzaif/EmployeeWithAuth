using AutoMapper;
using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.DraftDTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Entities;
using HRManagement.JwtFeatures;
using HRManagement.Models;
using HRManagement.Services.BlobStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRManagement.Services.Drafts
{
    public class DraftService : IDraftService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IJwtHandler _jwtHandler;
        private readonly IBlobStorageService _blobStorageService;
        private readonly string _containerNameForProfilePictures;
        private readonly IMapper _mapper;
        

        public DraftService(AppDbContext context, UserManager<User> userManager, IJwtHandler jwtHandler, IBlobStorageService blobStorageService, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _jwtHandler = jwtHandler;
            _blobStorageService = blobStorageService;
            _containerNameForProfilePictures = configuration["AzureBlobStorage:ProfilePictureContainerName"];
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateDraftAsync(EmployeeCreateDraftDTO draftdto, string usernameFromClaim)
        {

            var user = await _userManager.FindByNameAsync(usernameFromClaim);
            if (user == null)
            {
                return new ApiResponse(false, "Authentication error", 404, null);
            }


            var employee = _mapper.Map<Employee>(draftdto);
            employee.IsDraft = true;
            employee.CreatedBy = user.Id;
            employee.CreatedDate = DateTime.UtcNow;

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            var Draft = new ApiResponse
            {
                IsSuccess = true,
                Message = "Draft created successfully",
                StatusCode = 201,
                Response = employee // Replace with actual draft data if needed
            };
            return Draft;
        }

        public async Task<ApiResponse> GetDraftsAsync(string usernameFromClaim)
        {
            var user = await _userManager.FindByNameAsync(usernameFromClaim);
            if (user == null)
            {
                return new ApiResponse(false, "Authentication error", 404, null);
            }


            var drafts = await _context.Employees
                                .Where(e => e.IsDraft && e.CreatedBy == user.Id)
                                .ToListAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Drafts retrieved successfully",
                StatusCode = 200,
                Response = drafts
            };
        }

        public async Task<ApiResponse> UpdateDraftAsync(int id, string usernameFromClaim, EmployeeCreateDraftDTO updatedDraft)
        {
            var user = await _userManager.FindByNameAsync(usernameFromClaim);
            if (user == null)
            {
                return new ApiResponse(false, "Authentication error", 404, null);
            }


            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || !employee.IsDraft || employee.CreatedBy != user.Id)
                return new ApiResponse(false, "Draft not found or Unauthorized", 404, null);

            _mapper.Map(updatedDraft, employee);
            employee.ModifiedBy = user.Id;
            employee.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Draft updated and saved successfully",
                StatusCode = 200,
                Response = updatedDraft
            };
        }


        public async Task<ApiResponse> SubmitDraftAsync(int id, string usernameFromClaim, EmployeeCreateDTO finalizedDraft)
        {
            var adminUser = await _userManager.FindByNameAsync(usernameFromClaim);
            if (adminUser == null)
            {
                return new ApiResponse(false, "Authentication error", 404, null);
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || !employee.IsDraft || employee.CreatedBy != adminUser.Id)
                return new ApiResponse(false, "Draft not found or Unauthorized", 404, null);

            _mapper.Map(finalizedDraft, employee);
            employee.ModifiedBy = adminUser.Id;
            employee.ModifiedDate = DateTime.UtcNow;
            employee.IsDraft = false; // Mark as finalized  

            await _context.SaveChangesAsync();


            // Now doing with User table

            var user = new User
            {
                EmployeeName = finalizedDraft.EmployeeName,
                Email = finalizedDraft.WorkEmail,
                UserName = finalizedDraft.UserName,
                PhoneNumber = finalizedDraft.PersonalPhone
            };

            var result = await _userManager.CreateAsync(user, finalizedDraft.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);

                return new ApiResponse(false, "User registration failed: " + string.Join(", ", errors), 400, errors);

            }

            await _userManager.AddToRoleAsync(user, finalizedDraft.EmployeeRole);


            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Employee User created successfully",
                StatusCode = 200,
                Response = finalizedDraft
            };

        }

        public async Task<ApiResponse> DeleteDraftAsync(int id, string usernameFromClaim)
        {
            var user = await _userManager.FindByNameAsync(usernameFromClaim);
            if (user == null)
            {
                return new ApiResponse(false, "Authentication error", 404, null);
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || !employee.IsDraft || employee.CreatedBy != user.Id)
                return new ApiResponse(false, "Draft not found or Unathorized", 404, null);


            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Draft deleted successfully",
                StatusCode = 200,
                Response = employee
            };
        }



    }
}
