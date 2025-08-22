using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;

namespace HRManagement.Services.LeaveRequests
{
    public interface ILeaveRequestService
    {
        // User endpoints
        Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(string usernameFromClaim, GetLeaveRequestsForEmployeeFilterDto filters);
        Task<ApiResponse> CreateLeaveRequestAsync(CreateLeaveRequestDto dto, string usernameFromClaim);
        Task<ApiResponse> UpdateLeaveRequestAsync(int requestId, UpdateLeaveRequestDto dto, string usernameFromClaim);
        Task<ApiResponse> CancelLeaveRequestAsync(int requestId, string usernameFromClaim);
        Task<ApiResponse> GetLeaveBalancesForEmployeeAsync(string usernameFromClaim);
        Task<ApiResponse> GetUpcomingLeavesForEmployeeAsync(string usernameFromClaim);
        // Manager endpoints
        Task<ApiResponse> GetAllLeaveRequestsAsync(GetLeaveRequestsForAdminFilterDto filters);
        Task<ApiResponse> GetPendingLeaveRequests();
        Task<ApiResponse> GetPendingLeaveApprovalCountAsync();
        Task<ApiResponse> ApproveLeaveRequestAsync(int requestId, ApproveLeaveRequestDto dto);
        Task<ApiResponse> RejectLeaveRequestAsync(int requestId, RejectLeaveRequestDto dto);
        Task<ApiResponse> RevertApprovalAsync(int requestId, RemarksDto dto);
        Task<ApiResponse> GetEmployeesOnLeaveThisMonthAsync();
        Task<ApiResponse> GetCurrentPlannedLeavesAsync();
    }
}
