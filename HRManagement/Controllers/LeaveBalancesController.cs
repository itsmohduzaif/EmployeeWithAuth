using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveBalancesController : ControllerBase
    {
        private readonly ILeaveBalanceService _leaveBalanceService;

        public LeaveBalancesController(ILeaveBalanceService leaveBalanceService)
        {
            _leaveBalanceService = leaveBalanceService;
        }

        // GET api/leavebalances/{employeeId}
        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetLeaveBalancesForEmployee(int employeeId)
        {
            var Response = await _leaveBalanceService.GetLeaveBalancesByEmployeeIdAsync(employeeId);
            return Ok(Response);
        }

        //GET /employees/123/leave-balances/2
        [HttpGet("employees/{employeeId}/leave-balances/{leaveTypeId}")]
        public async Task<IActionResult> GetLeaveBalancesByEmployeeIdAndLeaveTypeIdAsync(int employeeId, int leaveTypeId)
        {
            var Response = await _leaveBalanceService.GetLeaveBalancesByEmployeeIdAndLeaveTypeIdAsync(employeeId, leaveTypeId);
            return Ok(Response);
        }

        // PUT api/leavebalances/{leaveBalanceId}
        [HttpPut("{leaveBalanceId}")]
        public async Task<IActionResult> UpdateLeaveBalance(int leaveBalanceId, [FromBody] UpdateLeaveBalanceDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                      .Select(e => e.ErrorMessage)
                                      .ToList();

                return BadRequest(new ApiResponse(false, "Body Validation failed", 400, errors));
            }

            var Response = await _leaveBalanceService.UpdateLeaveBalanceAsync(leaveBalanceId, dto);
            return Ok(Response);
        }
    }
}












//using HRManagement.Services.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace HRManagement.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class LeaveBalancesController : ControllerBase
//    {
//        private readonly ILeaveBalanceService _leaveBalanceService;

//        public LeaveBalancesController(ILeaveBalanceService leaveBalanceService)
//        {
//            _leaveBalanceService = leaveBalanceService;
//        }

//        [HttpGet("{employeeId}")]
//        public async Task<IActionResult> GetLeaveBalancesForEmployee(int employeeId)
//            => Ok(await _leaveBalanceService.GetLeaveBalancesByEmployeeIdAsync(employeeId));

//        [HttpPut("{balanceId}")]
//        public async Task<IActionResult> UpdateLeaveBalance(int balanceId, [FromBody] UpdateLeaveBalanceDto dto)
//            => Ok(await _leaveBalanceService.UpdateLeaveBalanceAsync(balanceId, dto));
//    }

//}
