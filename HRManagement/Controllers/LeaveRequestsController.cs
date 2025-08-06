using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
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
        public async Task<IActionResult> GetLeaveRequestsForEmployee()
        {
            //string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);   // Not using it for now (guid id)
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);

            var Response = await _leaveRequestService.GetLeaveRequestsForEmployeeAsync(usernameFromClaim);
            return Ok(Response);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDto dto)
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
        public async Task<IActionResult> UpdateLeaveRequest(int requestId, [FromBody] UpdateLeaveRequestDto dto)
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


        // Manager endpoints


        [Authorize(Roles = "Manager,Admin")]
        [HttpGet("pendingleaverequests")]
        public async Task<IActionResult> GetPendingLeaveRequests()
        {
            var response = await _leaveRequestService.GetPendingLeaveRequests();
            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "Manager,Admin")]
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

        [Authorize(Roles = "Manager,Admin")]
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
    }
}
