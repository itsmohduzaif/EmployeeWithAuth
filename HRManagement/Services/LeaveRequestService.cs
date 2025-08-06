using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Entities;
using HRManagement.Enums;
using HRManagement.Models.Leaves;
using HRManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HRManagement.JwtFeatures;
using Microsoft.AspNetCore.Identity;

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
        public async Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(string usernameFromClaim)
        {

            // Getting the employee from usernameFromClaim 
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
            }



            //var leaveRequests = await _context.LeaveRequests.FirstOrDefaultAsync(r => r.EmployeeId == employeeId);
            var leaveRequests = await _context.LeaveRequests
                .Where(r => r.EmployeeId == employee.EmployeeId)
                .OrderByDescending(r => r.RequestedOn)
                .ToListAsync();


            if (leaveRequests.Count == 0)             
            {
                return new ApiResponse(false, "No leave requests found for this employee.", 404, null);
            }

            return new ApiResponse(true, "Leave requests fetched successfully.", 200, leaveRequests);

        }


        //Create leave request(validation: overlapping/available balance)
        public async Task<ApiResponse> CreateLeaveRequestAsync(CreateLeaveRequestDto dto, string usernameFromClaim)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
            }

            int EmployeeId = employee.EmployeeId;

            // Validate dates
            if (dto.EndDate < dto.StartDate)
                return new ApiResponse(false, "End date can't be before start date.", 400, null);

            // Check for overlapping requests for this employee and leave type (Pending or Approved only)
            var overlapExists = await _context.LeaveRequests.AnyAsync(r =>
                r.EmployeeId == EmployeeId &&
                r.LeaveTypeId == dto.LeaveTypeId &&
                r.Status != LeaveRequestStatus.Rejected &&
                ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
                 (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
                 (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
            );
            // Checking for Pending or Approved but not for Rejected requests
            // New start date is inside an old leave,
            // New end date is inside an old leave,
            // New leave completely covers the old one


            if (overlapExists)
                return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);

            // Get relevant leave balance
            //var leaveBalance = await _context.LeaveBalances
            //    .FirstOrDefaultAsync(lb => lb.EmployeeId == dto.EmployeeId && lb.LeaveTypeId == dto.LeaveTypeId);

            //if (leaveBalance == null)
            //    return new ApiResponse(false, "Leave balance not initialized for this type.", 400, null);

            //int daysRequested = (int)(dto.EndDate - dto.StartDate).TotalDays + 1;
            //if ((leaveBalance.TotalAllocated - leaveBalance.Used) < daysRequested)
            //    return new ApiResponse(false, "Not enough leave balance.", 400, null);

            var leaveRequest = new LeaveRequest
            {
                EmployeeId = EmployeeId,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = LeaveRequestStatus.Pending,
                ManagerRemarks = null,
                RequestedOn = DateTime.UtcNow
            };

            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Leave request submitted.", 201, leaveRequest);
        }

        // Employee can update their own request if pending
        public async Task<ApiResponse> UpdateLeaveRequestAsync(int requestId, UpdateLeaveRequestDto dto, string usernameFromClaim)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
            }


            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null) {
                return new ApiResponse(false, "Request not found.", 404, null);
            }
            if (req.EmployeeId != employee.EmployeeId) { 
                return new ApiResponse(false, "You can only update your own requests.", 403, null);
            }
            if (req.Status != LeaveRequestStatus.Pending) {
                return new ApiResponse(false, "Can only modify pending requests.", 400, null);
            }

            // Optionally: again check overlaps if Start/EndDate changed
            req.StartDate = dto.StartDate;
            req.EndDate = dto.EndDate;
            req.Reason = dto.Reason;

            await _context.SaveChangesAsync();
            return new ApiResponse(true, "Request updated.", 200, req);
        }



        //Get all pending leave requests 
        public async Task<ApiResponse> GetPendingLeaveRequests()
        {
            // For demo: fetch all pending (customize with your reporting structure)
            var pending = await _context.LeaveRequests
                .Where(r => r.Status == LeaveRequestStatus.Pending)
                .OrderBy(r => r.StartDate)
                .ToListAsync();

            return new ApiResponse(true, "Pending requests fetched.", 200, pending);
        }

        


        // Manager approves and actioned/updates the leave balance
        public async Task<ApiResponse> ApproveLeaveRequestAsync(int requestId, ApproveLeaveRequestDto dto)
        {
            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null) return new ApiResponse(false, "Request not found.", 404, null);
            if (req.Status != LeaveRequestStatus.Pending)
                return new ApiResponse(false, "Cannot approve this request.", 400, null);

            // Extra: validate overlapping after updates or with new approvals

            int daysApproved = (int)(req.EndDate - req.StartDate).TotalDays + 1;
            Console.WriteLine("Days Approved are: ", daysApproved);

            //// Update leave balance
            //var leaveBalance = await _context.LeaveBalances.FirstOrDefaultAsync(lb => lb.EmployeeId == req.EmployeeId && lb.LeaveTypeId == req.LeaveTypeId);
            //if (leaveBalance == null)
            //    return new ApiResponse(false, "Leave balance not found.", 404, null);

            //if ((leaveBalance.TotalAllocated - leaveBalance.Used) < daysApproved)
            //    return new ApiResponse(false, "Leave balance insufficient for approval.", 400, null);

            //leaveBalance.Used += daysApproved;


            // Mark approved and update
            req.Status = LeaveRequestStatus.Approved;
            req.ManagerRemarks = dto?.ManagerRemarks ?? "";
            req.ActionedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Leave approved and balance updated.", 200, req);
        }

        public async Task<ApiResponse> RejectLeaveRequestAsync(int requestId, RejectLeaveRequestDto dto)
        {
            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null) return new ApiResponse(false, "Request not found.", 404, null);
            if (req.Status != LeaveRequestStatus.Pending)
                return new ApiResponse(false, "Cannot reject this request.", 400, null);

            req.Status = LeaveRequestStatus.Rejected;
            req.ManagerRemarks = dto?.ManagerRemarks ?? "";
            req.ActionedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Leave request rejected.", 200, req);
        }
    }
}
