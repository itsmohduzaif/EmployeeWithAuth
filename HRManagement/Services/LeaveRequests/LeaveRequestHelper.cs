using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.Enums;
using HRManagement.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Services.LeaveRequests
{
    public class LeaveRequestHelper : ILeaveRequestHelper
    {
        private readonly AppDbContext _context;
        public LeaveRequestHelper(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> CheckLeaveBalanceCoreAsync(int employeeId, int leaveTypeId, DateOnly startDate, DateOnly endDate,
            bool isStartDateHalfDay, bool isEndDateHalfDay, int? excludeRequestId=null)
        {
            if (leaveTypeId != 1 && leaveTypeId != 3 && leaveTypeId != 4)
            {
                // Get sum of leave days used for the employee, leave type, and approved requests in the same year
                var sumOfLeaveDaysUsed = await _context.LeaveRequests
                    .Where(r => r.EmployeeId == employeeId
                            && r.LeaveTypeId == leaveTypeId
                            && r.Status == LeaveRequestStatus.Approved
                            && r.StartDate.Year == startDate.Year
                            && r.LeaveRequestId != excludeRequestId)
                    .SumAsync(r => r.LeaveDaysUsed);

                Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

                // Get the leave type to get the default annual allocation
                var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
                if (leaveType == null)
                {
                    return new ApiResponse(false, "Leave type not found.", 404, null);
                }

                var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;


                // Calculate the requested leave days
                var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(startDate, endDate, isStartDateHalfDay, isEndDateHalfDay);

                Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

                // Check if the requested leave days, plus already used leave days, exceed the annual allocation
                if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
                {
                    return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
                }

                return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
            } // if ends here 
            else
            {
                // Get sum of leave days used for the employee, leave type, and approved requests in the same year
                var sumOfLeaveDaysUsed = await _context.LeaveRequests
                    .Where(r => r.EmployeeId == employeeId
                            && r.LeaveTypeId == 1
                            && r.Status == LeaveRequestStatus.Approved
                            && r.StartDate.Year == startDate.Year
                            && r.LeaveRequestId != excludeRequestId)
                    .SumAsync(r => r.LeaveDaysUsed) +
                                        await _context.LeaveRequests
                    .Where(r => r.EmployeeId == employeeId
                            && r.LeaveTypeId == 3
                            && r.Status == LeaveRequestStatus.Approved
                            && r.StartDate.Year == startDate.Year
                            && r.LeaveRequestId != excludeRequestId)
                    .SumAsync(r => r.LeaveDaysUsed) +
                                        await _context.LeaveRequests
                    .Where(r => r.EmployeeId == employeeId
                            && r.LeaveTypeId == 4
                            && r.Status == LeaveRequestStatus.Approved
                            && r.StartDate.Year == startDate.Year
                            && r.LeaveRequestId != excludeRequestId)
                    .SumAsync(r => r.LeaveDaysUsed);

                Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

                // Get the leave type to get the default annual allocation for Annual Leave (LeaveTypeId = 1)
                var leaveType = await _context.LeaveTypes.FindAsync(1);
                if (leaveType == null)
                {
                    return new ApiResponse(false, "Leave type not found.", 404, null);
                }

                var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

                // Calculate the requested leave days
                var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(startDate, endDate, isStartDateHalfDay, isEndDateHalfDay);

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
}
