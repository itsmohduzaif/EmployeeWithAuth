using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;

namespace HRManagement.Services.LeaveTypes
{
    public interface ILeaveTypeService
    {
        Task<ApiResponse> GetAllLeaveTypesAsync();
        Task<ApiResponse> GetLeaveTypeByIdAsync(int id);
        Task<ApiResponse> CreateLeaveTypeAsync(CreateLeaveTypeDto dto);
        Task<ApiResponse> UpdateLeaveTypeAsync(LeaveTypeDto dto);
    }

}
