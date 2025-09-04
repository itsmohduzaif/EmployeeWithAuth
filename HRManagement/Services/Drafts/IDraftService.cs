using HRManagement.DTOs;
using HRManagement.DTOs.DraftDTOs;
using HRManagement.DTOs.EmployeeDTOs;

namespace HRManagement.Services.Drafts
{
    public interface IDraftService
    {
        public Task<ApiResponse> CreateDraftAsync(EmployeeCreateDraftDTO draftdto, string usernameFromClaim);
        public Task<ApiResponse> GetDraftsAsync(string usernameFromClaim);
        public Task<ApiResponse> UpdateDraftAsync(int id, string usernameFromClaim, EmployeeCreateDraftDTO updatedDraft);
        public Task<ApiResponse> SubmitDraftAsync(int id, string usernameFromClaim, EmployeeCreateDTO finalizedDraft);
        public Task<ApiResponse> DeleteDraftAsync(int id, string usernameFromClaim);
    }
}
