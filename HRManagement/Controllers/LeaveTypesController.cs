using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.Services;
using HRManagement.Services.LeaveTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypesController : ControllerBase
    {
        private readonly ILeaveTypeService _LeaveTypeService;

        public LeaveTypesController(ILeaveTypeService LeaveTypeService)
        {
            _LeaveTypeService = LeaveTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLeaveTypesAsync()
        {
            var Response = await _LeaveTypeService.GetAllLeaveTypesAsync();
            return Ok(Response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeaveTypeByIdAsync(int id)
        {
            var Response = await _LeaveTypeService.GetLeaveTypeByIdAsync(id);
            return Ok(Response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateLeaveTypeAsync(CreateLeaveTypeDto dto)
        {
            var Response = await _LeaveTypeService.CreateLeaveTypeAsync(dto);
            return Ok(Response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateLeaveTypeAsync(LeaveTypeDto dto)
        {
            var Response = await _LeaveTypeService.UpdateLeaveTypeAsync(dto);
            return Ok(Response);
        }

    }
}