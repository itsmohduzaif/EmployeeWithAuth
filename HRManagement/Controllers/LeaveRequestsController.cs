using HRManagement.DTOs;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveRequestService;

        public LeaveRequestsController(ILeaveRequestService leaveRequestService)
        {
            _leaveRequestService = leaveRequestService;
        }

        // Employee endpoints

        [Authorize]
        [HttpGet("employee")]
        public async Task<IActionResult> GetLeaveRequestsForEmployee([FromBody] GetLeaveRequestsForEmployeeFilterDto filters)
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);   // Not using it for now (guid id)
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var Response = await _leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim, filters);
            return Ok(Response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromForm] CreateLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }

            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var response = await _leaveRequestService.CreateLeaveRequestAsync(dto, usernameFromClaim);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("{requestId}")]
        public async Task<IActionResult> UpdateLeaveRequest(int requestId, [FromForm] UpdateLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }

            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var response = await _leaveRequestService.UpdateLeaveRequestAsync(requestId, dto, usernameFromClaim);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpPut("{requestId}/cancel")]
        public async Task<IActionResult> CancelLeaveRequest(int requestId)
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var response = await _leaveRequestService.CancelLeaveRequestAsync(requestId, usernameFromClaim);
            return StatusCode(response.StatusCode, response);
        }


        [Authorize]
        [HttpGet("balances")]
        public async Task<IActionResult> GetLeaveBalancesForEmployee()
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var response = await _leaveRequestService.GetLeaveBalancesForEmployeeAsync(usernameFromClaim);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize]
        [HttpGet("upcoming-leaves")]
        public async Task<IActionResult> GetUpcomingLeaves()
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var result = await _leaveRequestService.GetUpcomingLeavesForEmployeeAsync(usernameFromClaim);
            return StatusCode(result.StatusCode, result);
        }



        // Admin endpoints

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLeaveRequests(GetLeaveRequestsForAdminFilterDto filters)
        {
            var response = await _leaveRequestService.GetAllLeaveRequestsAsync(filters);
            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("pendingleaverequests")]
        public async Task<IActionResult> GetPendingLeaveRequests()
        {
            var response = await _leaveRequestService.GetPendingLeaveRequests();
            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("pending-approvals-count")]
        public async Task<IActionResult> GetPendingApprovalCount()
        {
            var result = await _leaveRequestService.GetPendingLeaveApprovalCountAsync();
            return StatusCode(result.StatusCode, result);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{requestId}/approve")]
        public async Task<IActionResult> ApproveLeaveRequest(int requestId, [FromBody] ApproveLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }
            var response = await _leaveRequestService.ApproveLeaveRequestAsync(requestId, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{requestId}/reject")]
        public async Task<IActionResult> RejectLeaveRequest(int requestId, [FromBody] RejectLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }
            var response = await _leaveRequestService.RejectLeaveRequestAsync(requestId, dto);
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{requestId}/revert")]
        public async Task<IActionResult> RevertApproval(int requestId, [FromBody] RemarksDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }
            var response = await _leaveRequestService.RevertApprovalAsync(requestId, dto);
            return StatusCode(response.StatusCode, response);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("employees-on-leave-this-month")]
        public async Task<IActionResult> GetEmployeesOnLeaveThisMonth()
        {
            var result = await _leaveRequestService.GetEmployeesOnLeaveThisMonthAsync();
            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("current-planned-leaves")]
        public async Task<IActionResult> GetCurrentPlannedLeaves()
        {
            var result = await _leaveRequestService.GetCurrentPlannedLeavesAsync();
            return StatusCode(result.StatusCode, result);
        }


    }
}
