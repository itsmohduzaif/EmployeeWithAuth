using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.Models.Leaves;
using HRManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly AppDbContext _context;

        public LeaveBalanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> GetLeaveBalancesByEmployeeIdAsync(int employeeId)
        {
            var leaveBalancesByEmployeeId = await _context.LeaveBalances
                         .Where(lb => lb.EmployeeId == employeeId)
                         .ToListAsync();

            if (leaveBalancesByEmployeeId == null || leaveBalancesByEmployeeId.Count == 0)
            {
                return new ApiResponse(false, "No leave balances found for the employee.", 404, null);
            }

            return new ApiResponse(true, "Leave balances fetched successfully.", 200, leaveBalancesByEmployeeId);
        }

        public async Task<ApiResponse> GetLeaveBalancesByEmployeeIdAndLeaveTypeIdAsync(int employeeId, int leaveTypeId)
        {
            var leaveBalancesSearch = await _context.LeaveBalances
                         .FirstOrDefaultAsync(lb => lb.EmployeeId == employeeId && lb.LeaveTypeId == leaveTypeId);

            if (leaveBalancesSearch == null) {
                return new ApiResponse(false, "Leave balance not found for the specified employee and leave type.", 404, null);
            }

            return new ApiResponse(true, "Leave balance fetched successfully.", 200, leaveBalancesSearch);
        }

        public async Task<ApiResponse> UpdateLeaveBalanceAsync(int leaveBalanceId, UpdateLeaveBalanceDto dto)
        {
            var leaveBalance = await _context.LeaveBalances.FindAsync(leaveBalanceId);

            if (leaveBalance == null)
            {
                return new ApiResponse(false, "Leave balance not found.", 404, null);
            }

            leaveBalance.TotalAllocated = dto.TotalAllocated;
            leaveBalance.Used = dto.Used;

            _context.LeaveBalances.Update(leaveBalance);
            await _context.SaveChangesAsync();

            // Return updated DTO with Remaining
            var updatedDto = new LeaveBalanceDto
            {
                LeaveBalanceId = leaveBalance.LeaveBalanceId,
                EmployeeId = leaveBalance.EmployeeId,
                LeaveTypeId = leaveBalance.LeaveTypeId,
                TotalAllocated = leaveBalance.TotalAllocated,
                Used = leaveBalance.Used
            };

            return new ApiResponse(true, "Leave balance updated successfully.", 200, updatedDto);
        }
    }
}
