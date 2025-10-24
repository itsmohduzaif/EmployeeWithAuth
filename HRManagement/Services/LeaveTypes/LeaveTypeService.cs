using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.Models;
using HRManagement.Models.Leaves;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRManagement.Services.LeaveTypes
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly AppDbContext _context;

        public LeaveTypeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetAllLeaveTypesAsync()
        {
            var leaveTypes = await _context.LeaveTypes.ToListAsync();
            if (leaveTypes==null)
            {
                return new ApiResponse(false, "LeaveTypes not found", 404, null);
            }
            return new ApiResponse(true, "Fetched all leave types", 200, leaveTypes);
        }

        public async Task<ApiResponse?> GetLeaveTypeByIdAsync(int id)
        {
            var leaveType = await _context.LeaveTypes.FirstOrDefaultAsync(x => x.LeaveTypeId == id);
            if (leaveType == null)
            {
                return new ApiResponse(false, "LeaveType not found", 404, null);
            }
            return new ApiResponse(true, "Fetched leave types", 200, leaveType);
        }

        public async Task<ApiResponse> CreateLeaveTypeAsync(CreateLeaveTypeDto dto)
        {
            var leaveType = new LeaveType
            {
                LeaveTypeName = dto.LeaveTypeName,
                LeaveTypeDescription = dto.LeaveTypeDescription,
                DefaultAnnualAllocation = dto.DefaultAnnualAllocation
            };

            await _context.LeaveTypes.AddAsync(leaveType);
            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Leave Type created successfully",
                StatusCode = 201,
                Response = leaveType
            };
        }

        public async Task<ApiResponse> UpdateLeaveTypeAsync(LeaveTypeDto dto)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Leave Type not found!",
                    StatusCode = 404,
                    Response = null
                };
            }

            leaveType.LeaveTypeName = dto.LeaveTypeName;
            leaveType.LeaveTypeDescription = dto.LeaveTypeDescription;
            leaveType.DefaultAnnualAllocation = dto.DefaultAnnualAllocation;

            await _context.SaveChangesAsync();

            return new ApiResponse
            {
                IsSuccess = true,
                Message = "Leave Type updated successfully",
                StatusCode = 200,
                Response = leaveType
            };

        }
    }
}
