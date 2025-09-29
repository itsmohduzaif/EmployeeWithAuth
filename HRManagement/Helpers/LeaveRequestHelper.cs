    using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Helpers
{
    public class LeaveRequestHelper
    {
        private readonly AppDbContext _context;
        public LeaveRequestHelper(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CheckLeaveBalanceAsync(int employeeId, CreateLeaveRequestDto dto)
        {
            // Get sum of leave days used for the employee, leave type, and approved requests in the same year
            var sumOfLeaveDaysUsed = await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId
                        && r.LeaveTypeId == dto.LeaveTypeId
                        && r.Status == LeaveRequestStatus.Approved
                        && r.StartDate.Year == dto.StartDate.Year)
                .SumAsync(r => r.LeaveDaysUsed);

            Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

            // Get the leave type to get the default annual allocation
            var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                return new ApiResponse(false, "Leave type not found.", 404, null);
            }

            var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

            // Check if the total used leave days exceed the annual allocation
            if (sumOfLeaveDaysUsed >= defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
            }

            // Calculate the requested leave days
            var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(dto.StartDate, dto.EndDate);

            Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

            // Check if the requested leave days, plus already used leave days, exceed the annual allocation
            if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
            }

            return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        }



        public async Task<ApiResponse> CheckLeaveBalanceForUpdateLeaveRequestAsync(int employeeId, UpdateLeaveRequestDto dto)
        {
            // Get sum of leave days used for the employee, leave type, and approved requests in the same year
            var sumOfLeaveDaysUsed = await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId
                        && r.LeaveTypeId == dto.LeaveTypeId
                        && r.Status == LeaveRequestStatus.Approved
                        && r.StartDate.Year == dto.StartDate.Year)
                .SumAsync(r => r.LeaveDaysUsed);

            Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

            // Get the leave type to get the default annual allocation
            var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                return new ApiResponse(false, "Leave type not found.", 404, null);
            }

            var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

            // Check if the total used leave days exceed the annual allocation
            if (sumOfLeaveDaysUsed >= defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
            }

            // Calculate the requested leave days
            var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(dto.StartDate, dto.EndDate);

            Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

            // Check if the requested leave days, plus already used leave days, exceed the annual allocation
            if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
            }

            return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        }


    }
}
