using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Enums;
using HRManagement.Models.Leaves;
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

        public async Task<ApiResponse> CheckLeaveBalanceCoreAsync(int employeeId, int leaveTypeId, DateOnly startDate, DateOnly endDate, 
            bool isStartDateHalfDay, bool isEndDateHalfDay, int? excludeRequestId = null)
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




        // Commenting these three methods out as they are replaced by our generic CheckLeaveBalanceCoreAsync method above.

        //public async Task<ApiResponse> CheckLeaveBalanceAsync(int employeeId, CreateLeaveRequestDto dto)
        //{
        //    if (dto.LeaveTypeId != 1 && dto.LeaveTypeId != 3 && dto.LeaveTypeId != 4)
        //    {
        //        // Get sum of leave days used for the employee, leave type, and approved requests in the same year
        //        var sumOfLeaveDaysUsed = await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == dto.LeaveTypeId
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed);

        //        Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

        //        // Get the leave type to get the default annual allocation
        //        var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
        //        if (leaveType == null)
        //        {
        //            return new ApiResponse(false, "Leave type not found.", 404, null);
        //        }

        //        var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

        //        //// Check if the total used leave days exceed the annual allocation
        //        //if (sumOfLeaveDaysUsed >= defaultAnnualAllocation)
        //        //{
        //        //    return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
        //        //}

        //        // Calculate the requested leave days
        //        var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay);

        //        Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

        //        // Check if the requested leave days, plus already used leave days, exceed the annual allocation
        //        if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
        //        }

        //        return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        //    } // if ends here 
        //    else
        //    {
        //        // Get sum of leave days used for the employee, leave type, and approved requests in the same year
        //        var sumOfLeaveDaysUsed = await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == 1
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed) +
        //                                await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == 3
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed) +
        //                                await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == 4
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed);

        //        Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

        //        // Get the leave type to get the default annual allocation for Annual Leave (LeaveTypeId = 1)
        //        var leaveType = await _context.LeaveTypes.FindAsync(1);
        //        if (leaveType == null)
        //        {
        //            return new ApiResponse(false, "Leave type not found.", 404, null);
        //        }

        //        var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

        //        //// Check if the total used leave days exceed the annual allocation
        //        //if (sumOfLeaveDaysUsed >= defaultAnnualAllocation)
        //        //{
        //        //    return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
        //        //}

        //        // Calculate the requested leave days
        //        var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay);

        //        Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

        //        // Check if the requested leave days, plus already used leave days, exceed the annual allocation
        //        if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
        //        }

        //        return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        //    }






        //}






        //public async Task<ApiResponse> CheckLeaveBalanceForUpdateLeaveRequestAsync(int employeeId, UpdateLeaveRequestDto dto)
        //{
        //    if (dto.LeaveTypeId != 1 && dto.LeaveTypeId != 3 && dto.LeaveTypeId != 4)
        //    {
        //        // Get sum of leave days used for the employee, leave type, and approved requests in the same year
        //        var sumOfLeaveDaysUsed = await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == dto.LeaveTypeId
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed);

        //        Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

        //        // Get the leave type to get the default annual allocation
        //        var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
        //        if (leaveType == null)
        //        {
        //            return new ApiResponse(false, "Leave type not found.", 404, null);
        //        }

        //        var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

        //        // Check if the total used leave days exceed the annual allocation
        //        if (sumOfLeaveDaysUsed >= defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
        //        }

        //        // Calculate the requested leave days
        //        var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay);

        //        Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

        //        // Check if the requested leave days, plus already used leave days, exceed the annual allocation
        //        if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
        //        }

        //        return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        //    } // if ends here 
        //    else
        //    {
        //        // Get sum of leave days used for the employee, leave type, and approved requests in the same year
        //        var sumOfLeaveDaysUsed = await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == 1
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed) +
        //                                await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == 3
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed) +
        //                                await _context.LeaveRequests
        //            .Where(r => r.EmployeeId == employeeId
        //                    && r.LeaveTypeId == 4
        //                    && r.Status == LeaveRequestStatus.Approved
        //                    && r.StartDate.Year == dto.StartDate.Year)
        //            .SumAsync(r => r.LeaveDaysUsed);

        //        Console.WriteLine($"\n\n\n The sum of Leave Days Used for the Leave Type is: {sumOfLeaveDaysUsed}");

        //        // Get the leave type to get the default annual allocation for Annual Leave (LeaveTypeId = 1)
        //        var leaveType = await _context.LeaveTypes.FindAsync(1);
        //        if (leaveType == null)
        //        {
        //            return new ApiResponse(false, "Leave type not found.", 404, null);
        //        }

        //        var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

        //        // Check if the total used leave days exceed the annual allocation
        //        if (sumOfLeaveDaysUsed >= defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
        //        }

        //        // Calculate the requested leave days
        //        var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(dto.StartDate, dto.EndDate, dto.IsStartDateHalfDay, dto.IsEndDateHalfDay);

        //        Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

        //        // Check if the requested leave days, plus already used leave days, exceed the annual allocation
        //        if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
        //        }

        //        return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        //    }

        //}




        //public async Task<ApiResponse> CheckLeaveBalanceForApprovalOfLeaveRequestAsync(ApproveLeaveRequestDto dto, LeaveRequest req)
        //{
        //    if (req.LeaveTypeId != 1 && req.LeaveTypeId != 3 && req.LeaveTypeId != 4)
        //    {
        //        var sumOfLeaveDaysUsed = await _context.LeaveRequests
        //        .Where(r => r.EmployeeId == req.EmployeeId && r.LeaveTypeId == req.LeaveTypeId && r.Status == LeaveRequestStatus.Approved
        //        && r.StartDate.Year == req.StartDate.Year)
        //        .SumAsync(r => r.LeaveDaysUsed);

        //        var leaveType = await _context.LeaveTypes.FindAsync(req.LeaveTypeId);
        //        if (leaveType == null)
        //        {
        //            return new ApiResponse(false, "Leave type not found.", 404, null);
        //        }
        //        var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

        //        //int requestedLeaveDays = (req.EndDate - req.StartDate).Days + 1;
        //        var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(req.StartDate, req.EndDate, req.IsStartDateHalfDay, req.IsEndDateHalfDay);


        //        if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Leave balance is insufficient for approval.", 400, null);
        //        }

        //        return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        //    } // if ends here
        //    else
        //    {
        //        var sumOfLeaveDaysUsed = await _context.LeaveRequests
        //        .Where(r => r.EmployeeId == req.EmployeeId && r.LeaveTypeId == req.LeaveTypeId && r.Status == LeaveRequestStatus.Approved
        //        && r.StartDate.Year == req.StartDate.Year)
        //        .SumAsync(r => r.LeaveDaysUsed) +
        //                                await _context.LeaveRequests
        //        .Where(r => r.EmployeeId == req.EmployeeId && r.LeaveTypeId == req.LeaveTypeId && r.Status == LeaveRequestStatus.Approved
        //        && r.StartDate.Year == req.StartDate.Year)
        //        .SumAsync(r => r.LeaveDaysUsed) +
        //                                await _context.LeaveRequests
        //        .Where(r => r.EmployeeId == req.EmployeeId && r.LeaveTypeId == req.LeaveTypeId && r.Status == LeaveRequestStatus.Approved
        //        && r.StartDate.Year == req.StartDate.Year)
        //        .SumAsync(r => r.LeaveDaysUsed);

        //        var leaveType = await _context.LeaveTypes.FindAsync(1);
        //        if (leaveType == null)
        //        {
        //            return new ApiResponse(false, "Leave type not found.", 404, null);
        //        }
        //        var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

        //        //int requestedLeaveDays = (req.EndDate - req.StartDate).Days + 1;
        //        var requestedLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(req.StartDate, req.EndDate, req.IsStartDateHalfDay, req.IsEndDateHalfDay);


        //        Console.WriteLine($"\n\n\n {sumOfLeaveDaysUsed}  +   {requestedLeaveDays}    >      {defaultAnnualAllocation}");

        //        if (sumOfLeaveDaysUsed + requestedLeaveDays > defaultAnnualAllocation)
        //        {
        //            return new ApiResponse(false, "Leave balance is insufficient for approval.", 400, null);
        //        }

        //        return new ApiResponse(true, "Leave balance is sufficient.", 200, null);
        //    }

        //}












    }
}
