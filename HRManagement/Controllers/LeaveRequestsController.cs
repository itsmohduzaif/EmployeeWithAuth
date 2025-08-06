using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetLeaveRequestsForEmployee(int employeeId)
        {
            var Response = await _leaveRequestService.GetLeaveRequestsForEmployeeAsync(employeeId);
            return Ok(Response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest([FromBody] CreateLeaveRequestDto dto)
        {
            var response = await _leaveRequestService.CreateLeaveRequestAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{requestId}")]
        public async Task<IActionResult> UpdateLeaveRequest(int requestId, [FromBody] UpdateLeaveRequestDto dto)
        {
            var response = await _leaveRequestService.UpdateLeaveRequestAsync(requestId, dto);
            return StatusCode(response.StatusCode, response);
        }


        // Manager endpoints
        [HttpPut("{requestId}/approve")]
        public async Task<IActionResult> ApproveLeaveRequest(int requestId, [FromBody] ApproveLeaveRequestDto dto, [FromQuery] int managerId)
        {
            var response = await _leaveRequestService.ApproveLeaveRequestAsync(requestId, dto, managerId);
            return StatusCode(response.StatusCode, response);
        }


        
        [HttpGet("manager/{managerId}")]
        public async Task<IActionResult> GetPendingLeaveRequestsForManager(int managerId)
        {
            var response = await _leaveRequestService.GetPendingLeaveRequestsForManagerAsync(managerId);
            return StatusCode(response.StatusCode, response);
        }


        [HttpPut("{requestId}/reject")]
        public async Task<IActionResult> RejectLeaveRequest(int requestId, [FromBody] RejectLeaveRequestDto dto, [FromQuery] int managerId)
        {
            var response = await _leaveRequestService.RejectLeaveRequestAsync(requestId, dto, managerId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
