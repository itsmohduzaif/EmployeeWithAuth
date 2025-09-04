using HRManagement.DTOs;
using HRManagement.DTOs.DraftDTOs;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Models;
using HRManagement.Services.Drafts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DraftsController : ControllerBase
    {
        private readonly IDraftService _draftService;

        public DraftsController(IDraftService draftService)
        {
            _draftService = draftService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDraft(EmployeeCreateDraftDTO draftdto)
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usernameFromClaim))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));

            var result = await _draftService.CreateDraftAsync(draftdto, usernameFromClaim);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDrafts()
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usernameFromClaim))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));


            var result = await _draftService.GetDraftsAsync(usernameFromClaim);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDraft(int id, EmployeeCreateDraftDTO updatedDraft)
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usernameFromClaim))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));

            var result = await _draftService.UpdateDraftAsync(id, usernameFromClaim, updatedDraft);
            return Ok(result);
        }


        [HttpPut("submit/{id}")]
        public async Task<IActionResult> SubmitDraft(int id, EmployeeCreateDTO finalizedDraft)
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usernameFromClaim))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));

            var result = await _draftService.SubmitDraftAsync(id, usernameFromClaim, finalizedDraft);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDraft(int id)
        {
            string usernameFromClaim = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(usernameFromClaim))
                return Unauthorized(new ApiResponse(false, "User identity not found", 401, null));

            var result = await _draftService.DeleteDraftAsync(id, usernameFromClaim);
            return Ok(result);
        }


    }
}

// Also Todo
// code so that whenever a username is changed in the system, the draft created by that user should still be associated with the new username.