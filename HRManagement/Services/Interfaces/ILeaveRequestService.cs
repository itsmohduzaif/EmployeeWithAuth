using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;

namespace HRManagement.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        // User endpoints
        Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(string usernameFromClaim);
        Task<ApiResponse> CreateLeaveRequestAsync(CreateLeaveRequestDto dto, string usernameFromClaim);
        Task<ApiResponse> UpdateLeaveRequestAsync(int requestId, UpdateLeaveRequestDto dto, string usernameFromClaim);

        // Manager endpoints
        Task<ApiResponse> GetPendingLeaveRequests();
        Task<ApiResponse> ApproveLeaveRequestAsync(int requestId, ApproveLeaveRequestDto dto);
        Task<ApiResponse> RejectLeaveRequestAsync(int requestId, RejectLeaveRequestDto dto);
    }
}
