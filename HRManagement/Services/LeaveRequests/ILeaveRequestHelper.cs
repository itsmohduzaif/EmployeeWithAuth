using HRManagement.DTOs;

namespace HRManagement.Services.LeaveRequests
{
    public interface ILeaveRequestHelper
    {
        Task<ApiResponse> CheckLeaveBalanceCoreAsync(int employeeId, int leaveTypeId, DateOnly startDate, DateOnly endDate,
                                             bool isStartDateHalfDay, bool isEndDateHalfDay, int? excludeRequestId = null);


        //Task<ApiResponse> CheckLeaveBalanceCoreAsync(int employeeId, int leaveTypeId, DateOnly startDate, DateOnly endDate,
        //                                     bool isStartDateHalfDay, bool isEndDateHalfDay, int excludeRequestId);

    }
}
