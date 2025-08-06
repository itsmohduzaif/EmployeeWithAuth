using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;

namespace HRManagement.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        // User endpoints
        Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(int employeeId);
        Task<ApiResponse> CreateLeaveRequestAsync(CreateLeaveRequestDto dto);
        Task<ApiResponse> UpdateLeaveRequestAsync(int requestId, UpdateLeaveRequestDto dto);

        // Manager endpoints
        Task<ApiResponse> ApproveLeaveRequestAsync(int requestId, ApproveLeaveRequestDto dto, int managerId);
        Task<ApiResponse> GetPendingLeaveRequestsForManagerAsync(int managerId); // managerId could be fetched through context
        Task<ApiResponse> RejectLeaveRequestAsync(int requestId, RejectLeaveRequestDto dto, int managerId);
    }
}
