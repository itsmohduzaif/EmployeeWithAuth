using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Enums;
using HRManagement.Models.Leaves;
using HRManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRManagement.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly AppDbContext _context;

        public LeaveRequestService(AppDbContext context)
        {
            _context = context;
        }

        // Get all leave requests for one employee
        public async Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(int employeeId)
        {
            var leaveRequests = await _context.LeaveRequests.FirstOrDefaultAsync(r => r.EmployeeId == employeeId);

            if(leaveRequests == null)             
            {
                return new ApiResponse(false, "No leave requests found for this employee.", 404, null);
            }

            return new ApiResponse(true, "Leave requests fetched successfully.", 200, leaveRequests);

        }

        //Get all pending leave requests for manager's reportees (implement your own logic for manager-employee relationship)
        //public async Task<ApiResponse> GetPendingLeaveRequestsForManagerAsync(int managerId)
        //{
        //    // For demo: fetch all pending (customize with your reporting structure)
        //    var pending = await _context.LeaveRequests
        //        .Where(r => r.Status == LeaveRequestStatus.Pending)
        //        .OrderBy(r => r.StartDate)
        //        .ToListAsync();

        //    var dtos = pending.Select(r => MapToDto(r)).ToList();
        //    return new ApiResponse(true, "Pending requests fetched.", 200, dtos);
        //}

        // Create leave request (validation: overlapping/available balance)
        //public async Task<ApiResponse> CreateLeaveRequestAsync(CreateLeaveRequestDto dto)
        //{
        //    // Validate dates
        //    if (dto.EndDate < dto.StartDate)
        //        return new ApiResponse(false, "End date can't be before start date.", 400, null);

        //    // Check for overlapping requests for this employee and leave type (Pending or Approved only)
        //    var overlapExists = await _context.LeaveRequests.AnyAsync(r =>
        //        r.EmployeeId == dto.EmployeeId &&
        //        r.LeaveTypeId == dto.LeaveTypeId &&
        //        r.Status != LeaveRequestStatus.Rejected &&
        //        ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
        //         (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
        //         (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
        //    );
        //    if (overlapExists)
        //        return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);

        //    // Get relevant leave balance
        //    var leaveBalance = await _context.LeaveBalances
        //        .FirstOrDefaultAsync(lb => lb.EmployeeId == dto.EmployeeId && lb.LeaveTypeId == dto.LeaveTypeId);

        //    if (leaveBalance == null)
        //        return new ApiResponse(false, "Leave balance not initialized for this type.", 400, null);

        //    int daysRequested = (int)(dto.EndDate - dto.StartDate).TotalDays + 1;
        //    if ((leaveBalance.TotalAllocated - leaveBalance.Used) < daysRequested)
        //        return new ApiResponse(false, "Not enough leave balance.", 400, null);

        //    var leaveRequest = new LeaveRequest
        //    {
        //        EmployeeId = dto.EmployeeId,
        //        LeaveTypeId = dto.LeaveTypeId,
        //        StartDate = dto.StartDate,
        //        EndDate = dto.EndDate,
        //        Reason = dto.Reason,
        //        Status = LeaveRequestStatus.Pending,
        //        ManagerRemarks = null,
        //        RequestedOn = DateTime.UtcNow
        //    };

        //    await _context.LeaveRequests.AddAsync(leaveRequest);
        //    await _context.SaveChangesAsync();

        //    return new ApiResponse(true, "Leave request submitted.", 201, MapToDto(leaveRequest));
        //}

        //// Employee can update their own request if pending
        //public async Task<ApiResponse> UpdateLeaveRequestAsync(int requestId, UpdateLeaveRequestDto dto)
        //{
        //    var req = await _context.LeaveRequests.FindAsync(requestId);
        //    if (req == null) return new ApiResponse(false, "Request not found.", 404, null);
        //    if (req.Status != LeaveRequestStatus.Pending)
        //        return new ApiResponse(false, "Can only modify pending requests.", 400, null);

        //    // Optionally: again check overlaps if Start/EndDate changed
        //    req.StartDate = dto.StartDate;
        //    req.EndDate = dto.EndDate;
        //    req.Reason = dto.Reason;

        //    await _context.SaveChangesAsync();
        //    return new ApiResponse(true, "Request updated.", 200, MapToDto(req));
        //}

        //// Manager approves and actioned/updates the leave balance
        //public async Task<ApiResponse> ApproveLeaveRequestAsync(int requestId, ApproveLeaveRequestDto dto, int managerId)
        //{
        //    var req = await _context.LeaveRequests.FindAsync(requestId);
        //    if (req == null) return new ApiResponse(false, "Request not found.", 404, null);
        //    if (req.Status != LeaveRequestStatus.Pending)
        //        return new ApiResponse(false, "Cannot approve this request.", 400, null);

        //    // Extra: validate overlapping after updates or with new approvals

        //    int daysApproved = (int)(req.EndDate - req.StartDate).TotalDays + 1;

        //    // Update leave balance
        //    var leaveBalance = await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.EmployeeId == req.EmployeeId && lb.LeaveTypeId == req.LeaveTypeId);
        //    if (leaveBalance == null)
        //        return new ApiResponse(false, "Leave balance not found.", 404, null);

        //    if ((leaveBalance.TotalAllocated - leaveBalance.Used) < daysApproved)
        //        return new ApiResponse(false, "Leave balance insufficient for approval.", 400, null);

        //    leaveBalance.Used += daysApproved;

        //    // Mark approved and update
        //    req.Status = LeaveRequestStatus.Approved;
        //    req.ManagerRemarks = dto?.ManagerRemarks ?? "";
        //    req.ActionedOn = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();

        //    return new ApiResponse(true, "Leave approved and balance updated.", 200, MapToDto(req));
        //}

        //public async Task<ApiResponse> RejectLeaveRequestAsync(int requestId, RejectLeaveRequestDto dto, int managerId)
        //{
        //    var req = await _context.LeaveRequests.FindAsync(requestId);
        //    if (req == null) return new ApiResponse(false, "Request not found.", 404, null);
        //    if (req.Status != LeaveRequestStatus.Pending)
        //        return new ApiResponse(false, "Cannot reject this request.", 400, null);

        //    req.Status = LeaveRequestStatus.Rejected;
        //    req.ManagerRemarks = dto?.ManagerRemarks ?? "";
        //    req.ActionedOn = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();

        //    return new ApiResponse(true, "Leave request rejected.", 200, MapToDto(req));
        //}

        //// Helper/mapper method
        //private LeaveRequestDto MapToDto(LeaveRequest r)
        //{
        //    return new LeaveRequestDto
        //    {
        //        LeaveRequestId = r.LeaveRequestId,
        //        EmployeeId = r.EmployeeId,
        //        LeaveTypeId = r.LeaveTypeId,
        //        StartDate = r.StartDate,
        //        EndDate = r.EndDate,
        //        Reason = r.Reason,
        //        Status = r.Status.ToString(),
        //        ManagerRemarks = r.ManagerRemarks,
        //        RequestedOn = r.RequestedOn,
        //        ActionedOn = r.ActionedOn
        //    };
        //}
    }
}
