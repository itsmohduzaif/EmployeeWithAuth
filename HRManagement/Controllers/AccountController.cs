using HRManagement.DTOs;
using HRManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST https://localhost:7150/api/Account/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTOs.Register model)
        {
            var Response = await _accountService.Register(model);
            return Ok(Response);
        }

        // POST https://localhost:7150/api/Account/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTOs.Login model)
        {
            var Response = await _accountService.Login(model);
            return Ok(Response);
        }

        // POST https://localhost:7150/api/Account/add-role
        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string role)
        {
            var Response = await _accountService.AddRole(role);
            return Ok(Response);
        }

        // POST https://localhost:7150/api/Account/assign-role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole model)
        {
            var Response = await _accountService.AssignRole(model);
            return Ok(Response);
        }
    }
}
