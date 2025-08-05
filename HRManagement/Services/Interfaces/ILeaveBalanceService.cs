using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;

namespace HRManagement.Services.Interfaces
{
    public interface ILeaveBalanceService
    {
        Task<ApiResponse> GetLeaveBalancesByEmployeeIdAsync(int employeeId);
        Task<ApiResponse> GetLeaveBalancesByEmployeeIdAndLeaveTypeIdAsync(int employeeId, int leaveTypeId);
        Task<ApiResponse> UpdateLeaveBalanceAsync(int leaveBalanceId, UpdateLeaveBalanceDto dto);
    }
}
