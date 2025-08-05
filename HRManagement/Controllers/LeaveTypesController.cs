using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.Services;
using HRManagement.Services.Interfaces;
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

        [HttpPost]
        public async Task<IActionResult> CreateLeaveTypeAsync(CreateLeaveTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }

            var Response = await _LeaveTypeService.CreateLeaveTypeAsync(dto);
            return Ok(Response);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLeaveTypeAsync(LeaveTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }

            var Response = await _LeaveTypeService.UpdateLeaveTypeAsync(dto);
            return Ok(Response);
        }

    }
}
