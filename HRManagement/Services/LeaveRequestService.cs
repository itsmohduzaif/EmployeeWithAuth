using Azure.Core;
using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Enums;
using HRManagement.Models;
using HRManagement.Models.Leaves;
using HRManagement.Services.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace HRManagement.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly AppDbContext _context;
        private readonly string _containerNameForLeaveRequestFiles;
        private readonly BlobStorageService _blobStorageService;

        public LeaveRequestService(AppDbContext context, IConfiguration configuration, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _containerNameForLeaveRequestFiles = configuration["AzureBlobStorage:LeaveRequestFilesContainerName"];
        }

        public async Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(string usernameFromClaim)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == usernameFromClaim);

            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found", 404, null);
            }

            var leaveRequests = await _context.LeaveRequests
                .Where(r => r.EmployeeId == employee.EmployeeId)
                .OrderByDescending(r => r.RequestedOn)
                .ToListAsync();

            if (!leaveRequests.Any())
            {
                return new ApiResponse(false, "No leave requests found.", 404, null);
            }

            var responseDtos = leaveRequests.Select(lr => new GetLeaveRequestsForEmployeeDto
            {
                LeaveRequestId = lr.LeaveRequestId,
                EmployeeId = lr.EmployeeId,
                LeaveTypeId = lr.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                ManagerRemarks = lr.ManagerRemarks,
                RequestedOn = lr.RequestedOn,
                ActionedOn = lr.ActionedOn,
                LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
                TemporaryBlobUrls = lr.LeaveRequestFileNames?
                    .Where(fileName => !string.IsNullOrEmpty(fileName))
                    .Select(fileName => _blobStorageService.GetTemporaryBlobUrl(fileName, _containerNameForLeaveRequestFiles))
                    .ToList()
            }).ToList();

            return new ApiResponse(true, "Leave requests fetched successfully.", 200, responseDtos);
        }


        


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
                r.Status != LeaveRequestStatus.Rejected &&
                r.Status != LeaveRequestStatus.Cancelled && 
                ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
                 (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
                 (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
            );      //r.LeaveTypeId == dto.LeaveTypeId &&    removed this condition to allow checking overlapping of different leave types

            if (overlapExists)
                return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);





            // Code to check leave balance
            var sumOfUsedLeaves = await _context.LeaveRequests
                .Where(r => r.EmployeeId == EmployeeId && r.LeaveTypeId == dto.LeaveTypeId && r.Status == LeaveRequestStatus.Approved)
                .SumAsync(r => EF.Functions.DateDiffDay(r.StartDate, r.EndDate) + 1);

            // Get the default annual allocation for this leave type
            var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                return new ApiResponse(false, "Leave type not found.", 404, null);
            }

            var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

            if (sumOfUsedLeaves >= defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
            }

            // Calculate the number of days for the current leave request
            int requestedLeaveDays = (dto.EndDate - dto.StartDate).Days + 1;

            if (sumOfUsedLeaves + requestedLeaveDays > defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
            }






            // Prepare to store file names
            var fileNames = new List<string>();

            // Validate multiple file uploads
            if (dto.Files != null && dto.Files.Any())
            {
                // Allowed file extensions for leave request
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".txt" };
                var allowedContentTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/jpg", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };


                foreach (var file in dto.Files)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return new ApiResponse(false, "Only PDF, JPG, JPEG, PNG, DOCX, DOC, and TXT file formats are allowed.", 400, null);
                    }

                    // Check content type
                    if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
                    {
                        return new ApiResponse(false, "Invalid file type. Only PDF, JPG, JPEG, PNG, DOCX, DOC, and TXT formats are allowed.", 400, null);
                    }

                    // Check file size (e.g., max 10 MB)
                    var maxFileSize = 5 * 1024 * 1024; // 10 MB in bytes
                    if (file.Length > maxFileSize)
                    {
                        return new ApiResponse(false, "One of the files exceeds the maximum allowed size of 10 MB.", 400, null);
                    }

                    // Generate a unique file name for each file
                    var uniqueFileName = $"{usernameFromClaim}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                    // Upload the file to Azure Blob Storage
                    string blobName = await _blobStorageService.UploadFileAsync(file, uniqueFileName, _containerNameForLeaveRequestFiles);
                    fileNames.Add(blobName); // Collect the blob names
                }
            }



            // Store the file names as a List<string> (LeaveRequestFileNames)
            var leaveRequest = new LeaveRequest
            {
                EmployeeId = EmployeeId,
                LeaveTypeId = dto.LeaveTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = LeaveRequestStatus.Pending,
                ManagerRemarks = null,
                RequestedOn = DateTime.UtcNow,
                LeaveRequestFileNames = fileNames // Store the file names as a List<string>
            };

            await _context.LeaveRequests.AddAsync(leaveRequest);
            await _context.SaveChangesAsync();

            return new ApiResponse(true, "Leave request submitted.", 201, leaveRequest);
        }

















        // Commenting this as the date validation functionality is not completed yet.
        // Employee can update their own request if pending
        public async Task<ApiResponse> UpdateLeaveRequestAsync(int requestId, UpdateLeaveRequestDto dto, string usernameFromClaim)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
            }


            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null)
            {
                return new ApiResponse(false, "Request not found.", 404, null);
            }
            if (req.EmployeeId != employee.EmployeeId)
            {
                return new ApiResponse(false, "You can only update your own requests.", 403, null);
            }
            if (req.Status != LeaveRequestStatus.Pending)
            {
                return new ApiResponse(false, "Can only modify pending requests.", 400, null);
            }





            int EmployeeId = employee.EmployeeId;

            // Validate dates
            if (dto.EndDate < dto.StartDate)
                return new ApiResponse(false, "End date can't be before start date.", 400, null);

            // Check for overlapping requests for this employee and leave type (Pending or Approved only)
            var overlapExists = await _context.LeaveRequests.AnyAsync(r =>
                r.EmployeeId == EmployeeId &&
                r.LeaveRequestId != requestId && // Exclude the current request being updated
                r.Status != LeaveRequestStatus.Rejected &&
                r.Status != LeaveRequestStatus.Cancelled &&
                ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
                 (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
                 (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
            );      //r.LeaveTypeId == dto.LeaveTypeId &&    removed this condition to allow checking overlapping of different leave types

            if (overlapExists)
                return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);





            // Code to check leave balance
            var sumOfUsedLeaves = await _context.LeaveRequests
                .Where(lr => lr.EmployeeId == EmployeeId && lr.LeaveTypeId == dto.LeaveTypeId && lr.Status == LeaveRequestStatus.Approved)
                .SumAsync(lr => EF.Functions.DateDiffDay(lr.StartDate, lr.EndDate) + 1);

            // Get the default annual allocation for this leave type
            var leaveType = await _context.LeaveTypes.FindAsync(dto.LeaveTypeId);
            if (leaveType == null)
            {
                return new ApiResponse(false, "Leave type not found.", 404, null);
            }

            var defaultAnnualAllocation = leaveType.DefaultAnnualAllocation;

            if (sumOfUsedLeaves >= defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Leave balance exceeded for this leave type.", 400, null);
            }

            // Calculate the number of days for the current leave request
            int requestedLeaveDays = (dto.EndDate - dto.StartDate).Days + 1;

            if (sumOfUsedLeaves + requestedLeaveDays > defaultAnnualAllocation)
            {
                return new ApiResponse(false, "Insufficient leave balance for this request.", 400, null);
            }








            ////....................................... File Updation Logic .......................................

            // Deleting the old files
            if (req.LeaveRequestFileNames != null && req.LeaveRequestFileNames.Any())
            {
                foreach (var fileName in req.LeaveRequestFileNames)
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        try
                        {
                            await _blobStorageService.DeleteFileAsync(fileName, _containerNameForLeaveRequestFiles);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                    }
                }
            }

            req.LeaveRequestFileNames = []; // Reset the file names list

            // Deletion complete of the old files.



            var fileNames = new List<string>();

            // Validate multiple file uploads
            if (dto.Files != null && dto.Files.Any())
            {
                // Allowed file extensions for leave request
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".txt" };
                var allowedContentTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/jpg", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };


                foreach (var file in dto.Files)
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return new ApiResponse(false, "Only PDF, JPG, JPEG, PNG, DOCX, DOC, and TXT file formats are allowed.", 400, null);
                    }

                    // Check content type
                    if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
                    {
                        return new ApiResponse(false, "Invalid file type. Only PDF, JPG, JPEG, PNG, DOCX, DOC, and TXT formats are allowed.", 400, null);
                    }

                    // Check file size (e.g., max 10 MB)
                    var maxFileSize = 5 * 1024 * 1024; // 10 MB in bytes
                    if (file.Length > maxFileSize)
                    {
                        return new ApiResponse(false, "One of the files exceeds the maximum allowed size of 10 MB.", 400, null);
                    }

                    // Generate a unique file name for each file
                    var uniqueFileName = $"{usernameFromClaim}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                    // Upload the file to Azure Blob Storage
                    string blobName = await _blobStorageService.UploadFileAsync(file, uniqueFileName, _containerNameForLeaveRequestFiles);
                    fileNames.Add(blobName); // Collect the blob names
                }
            }

            ////.......................................File Updation Logic End.......................................



            // Optionally: again check overlaps if Start/EndDate changed
            req.StartDate = dto.StartDate;
            req.EndDate = dto.EndDate;
            req.Reason = dto.Reason;
            req.LeaveTypeId = dto.LeaveTypeId;
            req.LeaveRequestFileNames = fileNames; // Update with new file names

            await _context.SaveChangesAsync();
            return new ApiResponse(true, "Request updated.", 200, req);
        }


        public async Task<ApiResponse> CancelLeaveRequestAsync(int requestId, string usernameFromClaim)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
            }

            var request = await _context.LeaveRequests.FindAsync(requestId);
            if (request == null || request.EmployeeId != employee.EmployeeId)
                return new ApiResponse(false, "Leave request not found or access denied.", 403, null);

            if (request.Status != LeaveRequestStatus.Pending)
                return new ApiResponse(false, "Only pending requests can be withdrawn.", 400, null);

            request.Status = LeaveRequestStatus.Cancelled;
            request.ActionedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return new ApiResponse(true, "Leave request withdrawn successfully.", 200, request);
        }






        public async Task<ApiResponse> GetLeaveBalancesForEmployeeAsync(string usernameFromClaim)
        {

            Console.WriteLine($"\n\n\n\n{DateTime.UtcNow.Year}");
            Console.WriteLine($"\n\n\n\n{DateTime.UtcNow}");

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
            if (employee == null)
                return new ApiResponse(false, "Employee not found.", 404, null);

            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            var balances = new List<LeaveBalanceDto>();

            foreach (var lt in leaveTypes)
            {
                // Only count approved leaves for this employee & leave type, and for the current year
                var used = await _context.LeaveRequests
                    .Where(r => r.EmployeeId == employee.EmployeeId
                             && r.LeaveTypeId == lt.LeaveTypeId
                             && r.Status == LeaveRequestStatus.Approved
                             && r.StartDate.Year == DateTime.UtcNow.Year) // If annual allocation
                    .SumAsync(r => EF.Functions.DateDiffDay(r.StartDate, r.EndDate) + 1);


                balances.Add(new LeaveBalanceDto
                {
                    LeaveTypeId = lt.LeaveTypeId,
                    LeaveTypeName = lt.LeaveTypeName,
                    DefaultAnnualAllocation = lt.DefaultAnnualAllocation,
                    Used = used,
                    Remaining = lt.DefaultAnnualAllocation - used
                });
            }

            //DateTime dateTime = DateTime.UtcNow;
            DateTime dateTime1 = new DateTime(2025, 7, 17, 0, 0, 0, DateTimeKind.Utc);
            DateTime dateTime2 = new DateTime(2025, 7, 18, 0, 0, 0, DateTimeKind.Utc);

            Console.WriteLine(dateTime1 - dateTime2);

            return new ApiResponse(true, "Leave balances fetched.", 200, balances);
        }



        // Only shows for upcoming leaves (approved) for the employee
        public async Task<ApiResponse> GetUpcomingLeavesForEmployeeAsync(string usernameFromClaim)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Username == usernameFromClaim);

            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found", 404, null);
            }

            var upcomingLeaves = await _context.LeaveRequests
                .Where(r => r.EmployeeId == employee.EmployeeId
                            && r.Status == LeaveRequestStatus.Approved
                            && r.StartDate >= DateTime.UtcNow.Date)
                .OrderBy(r => r.StartDate)
                .ToListAsync();

            if (!upcomingLeaves.Any())
            {
                return new ApiResponse(false, "No upcoming leaves found.", 404, null);
            }

            var responseDtos = upcomingLeaves.Select(lr => new GetLeaveRequestsForEmployeeDto
            {
                LeaveRequestId = lr.LeaveRequestId,
                EmployeeId = lr.EmployeeId,
                LeaveTypeId = lr.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                ManagerRemarks = lr.ManagerRemarks,
                RequestedOn = lr.RequestedOn,
                ActionedOn = lr.ActionedOn,
                LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
                TemporaryBlobUrls = lr.LeaveRequestFileNames?
                    .Where(fileName => !string.IsNullOrEmpty(fileName))
                    .Select(fileName => _blobStorageService.GetTemporaryBlobUrl(fileName, _containerNameForLeaveRequestFiles))
                    .ToList()
            }).ToList();

            return new ApiResponse(true, "Upcoming leaves fetched successfully.", 200, responseDtos);
        }


























        // Manager approves and actioned/updates the leave balance


        public async Task<ApiResponse> GetAllLeaveRequestsAsync()
        {
            //var allRequests = await _context.LeaveRequests
            //    .Include(r => r.Employee)       // Optional: to include employee details
            //    .Include(r => r.LeaveType)      // Optional: to include leave type
            //    .OrderByDescending(r => r.RequestedOn)
            //    .ToListAsync();

            var allRequests = await _context.LeaveRequests
                .OrderByDescending(r => r.RequestedOn)
                .ToListAsync();


            if (!allRequests.Any())
            {
                return new ApiResponse(false, "No leave requests found.", 404, null);
            }


            var responseDtos = allRequests.Select(lr => new GetLeaveRequestsForEmployeeDto
            {
                LeaveRequestId = lr.LeaveRequestId,
                EmployeeId = lr.EmployeeId,
                LeaveTypeId = lr.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                ManagerRemarks = lr.ManagerRemarks,
                RequestedOn = lr.RequestedOn,
                ActionedOn = lr.ActionedOn,
                LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
                TemporaryBlobUrls = lr.LeaveRequestFileNames?
                    .Where(fileName => !string.IsNullOrEmpty(fileName))
                    .Select(fileName => _blobStorageService.GetTemporaryBlobUrl(fileName, _containerNameForLeaveRequestFiles))
                    .ToList()
            }).ToList();

            return new ApiResponse(true, "Leave requests fetched successfully.", 200, responseDtos);



            //return new ApiResponse(true, "All leave requests fetched successfully.", 200, allRequests);
        }



        //Get all pending leave requests 
        public async Task<ApiResponse> GetPendingLeaveRequests()
        {
            // For demo: fetch all pending (customize with your reporting structure)
            var pendingRequests = await _context.LeaveRequests
                .Where(r => r.Status == LeaveRequestStatus.Pending)
                .OrderBy(r => r.StartDate)
                .ToListAsync();

            if (!pendingRequests.Any())
            {
                return new ApiResponse(false, "No pending leave requests found.", 404, null);
            }

            var responseDtos = pendingRequests.Select(lr => new GetLeaveRequestsForEmployeeDto
            {
                LeaveRequestId = lr.LeaveRequestId,
                EmployeeId = lr.EmployeeId,
                LeaveTypeId = lr.LeaveTypeId,
                StartDate = lr.StartDate,
                EndDate = lr.EndDate,
                Reason = lr.Reason,
                Status = lr.Status,
                ManagerRemarks = lr.ManagerRemarks,
                RequestedOn = lr.RequestedOn,
                ActionedOn = lr.ActionedOn,
                LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
                TemporaryBlobUrls = lr.LeaveRequestFileNames?
                    .Where(fileName => !string.IsNullOrEmpty(fileName))
                    .Select(fileName => _blobStorageService.GetTemporaryBlobUrl(fileName, _containerNameForLeaveRequestFiles))
                    .ToList()
            }).ToList();

            return new ApiResponse(true, "Leave requests fetched successfully.", 200, responseDtos);




            //return new ApiResponse(true, "Pending requests fetched.", 200, pending);
        }



        public async Task<ApiResponse> GetPendingLeaveApprovalCountAsync()
        {
            int count = await _context.LeaveRequests
                .CountAsync(r => r.Status == LeaveRequestStatus.Pending);

            return new ApiResponse(true, "Pending leave approval count fetched successfully.", 200, count);
        }




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


        public async Task<ApiResponse> RevertApprovalAsync(int requestId, RemarksDto dto)
        {
            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null) return new ApiResponse(false, "Request not found.", 404, null);

            if (req.Status == LeaveRequestStatus.Approved)
            {
                req.Status = LeaveRequestStatus.Pending;
            }
            else if (req.Status == LeaveRequestStatus.Rejected)
            {
                req.Status = LeaveRequestStatus.Pending;
            }
            else
            {
                return new ApiResponse(false, "Only approved or rejected requests can be reverted.", 400, null);
            }

            req.ManagerRemarks = dto?.ManagerRemarks ?? "";
            req.ActionedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return new ApiResponse(true, "Revert Success.", 200, req);

        }


        //var responseDtos = leaveRequests.Select(lr => new GetLeaveRequestsForEmployeeDto
        //{
        //    LeaveRequestId = lr.LeaveRequestId,
        //    EmployeeId = lr.EmployeeId,
        //    LeaveTypeId = lr.LeaveTypeId,
        //    StartDate = lr.StartDate,
        //    EndDate = lr.EndDate,
        //    Reason = lr.Reason,
        //    Status = lr.Status,
        //    ManagerRemarks = lr.ManagerRemarks,
        //    RequestedOn = lr.RequestedOn,
        //    ActionedOn = lr.ActionedOn,
        //    LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
        //    TemporaryBlobUrls = lr.LeaveRequestFileNames?
        //            .Where(fileName => !string.IsNullOrEmpty(fileName))
        //            .Select(fileName => _blobStorageService.GetTemporaryBlobUrl(fileName, _containerNameForLeaveRequestFiles))
        //            .ToList()
        //}).ToList();

        //// Validate dates
        //    if (dto.EndDate<dto.StartDate)
        //        return new ApiResponse(false, "End date can't be before start date.", 400, null);

        //// Check for overlapping requests for this employee and leave type (Pending or Approved only)
        //var overlapExists = await _context.LeaveRequests.AnyAsync(r =>
        //    r.EmployeeId == EmployeeId &&
        //    r.Status != LeaveRequestStatus.Rejected &&
        //    r.Status != LeaveRequestStatus.Cancelled &&
        //    ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
        //     (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
        //     (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
        //);      //r.LeaveTypeId == dto.LeaveTypeId &&    removed this condition to allow checking overlapping of different leave types

        //    if (overlapExists)
        //        return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);




        // the lr which are approved and for this month
        public async Task<ApiResponse> GetEmployeesOnLeaveThisMonthAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            //Start of Month: 01 - 08 - 2025 00:00:00, End of Month: 31 - 08 - 2025 00:00:00
            Console.WriteLine($"\n\n\n\n\n\nStart of Month: {startOfMonth}, End of Month: {endOfMonth}");

            //startOfMonth.AddMonths(1): 01 - 09 - 2025 00:00:00, startOfMonth.AddMonths(1).AddDays(1): 02 - 09 - 2025 00:00:00
            Console.WriteLine($"\n\n\n\n\n\nstartOfMonth.AddMonths(1): {startOfMonth.AddMonths(1)}, startOfMonth.AddMonths(1).AddDays(1): {startOfMonth.AddMonths(1).AddDays(1)}");


            var approvedLeaveRequestsOfThisMonth = await _context.LeaveRequests
                .Where(lr => lr.Status == LeaveRequestStatus.Approved &&
                (lr.StartDate >= startOfMonth && lr.EndDate <= endOfMonth) ||
                (lr.StartDate >= startOfMonth && lr.StartDate <= endOfMonth) ||
                (lr.EndDate == startOfMonth) ||
                (lr.StartDate == endOfMonth) ||
                (lr.StartDate <= endOfMonth && lr.EndDate >= startOfMonth))
                .OrderBy(r => r.StartDate)
                .ToListAsync();


            if (!approvedLeaveRequestsOfThisMonth.Any())
            {
                return new ApiResponse(false, "No leave requests found.", 404, null);
            }


            var responseDtos = new List<object>();

            foreach (var lr in approvedLeaveRequestsOfThisMonth)
            {
                var employee = await _context.Employees.FindAsync(lr.EmployeeId);

                if (employee == null)
                {
                    // If employee not found, skip this leave request
                    continue;
                }


                responseDtos.Add(new
                {
                    LeaveRequestId = lr.LeaveRequestId,
                    EmployeeId = lr.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Username = employee.Username,
                    Email = employee.Email,
                    LeaveTypeId = lr.LeaveTypeId,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    Status = lr.Status,
                    ManagerRemarks = lr.ManagerRemarks,
                    RequestedOn = lr.RequestedOn,
                    ActionedOn = lr.ActionedOn,
                    LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
                    TemporaryBlobUrls = lr.LeaveRequestFileNames?
                        .Where(fileName => !string.IsNullOrEmpty(fileName))
                        .Select(fileName => _blobStorageService.GetTemporaryBlobUrl(fileName, _containerNameForLeaveRequestFiles))
                        .ToList()
                });
            }




            //return new ApiResponse(true, "Employees on leave this month fetched successfully.", 200, approvedLeaveRequestsOfThisMonth);
            return new ApiResponse(true, "Employees on leave this month fetched successfully.", 200, responseDtos);
            //return new ApiResponse(true, "Employees on leave this month fetched successfully.", 200, null);

        }



        // Current Planned Leaves Employees
        // Showing the employees who are currently on leave today (i.e., today falls between their approved leave start and end dates).
        public async Task<ApiResponse> GetCurrentPlannedLeavesAsync()
        {
            var today = DateTime.UtcNow.Date;
            Console.WriteLine($"Today: {today}");
            var currentLeavesToday = await _context.LeaveRequests
                .Where(lr => lr.Status == LeaveRequestStatus.Approved &&
                 (lr.StartDate <= today && lr.EndDate >= today))
                .OrderBy(lr => lr.StartDate)
                .ToListAsync();


            if (!currentLeavesToday.Any())
            {
                return new ApiResponse(false, "No leave requests found.", 404, null);
            }


            var responseDtos = new List<object>();

            foreach (var lr in currentLeavesToday)
            {
                var employee = await _context.Employees.FindAsync(lr.EmployeeId);

                if (employee == null)
                {
                    // If employee not found, skip this leave request
                    continue;
                }


                responseDtos.Add(new
                {
                    LeaveRequestId = lr.LeaveRequestId,
                    EmployeeId = lr.EmployeeId,
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Username = employee.Username,
                    Email = employee.Email,
                    LeaveTypeId = lr.LeaveTypeId,
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    Reason = lr.Reason,
                    Status = lr.Status,
                    ManagerRemarks = lr.ManagerRemarks,
                    RequestedOn = lr.RequestedOn,
                    ActionedOn = lr.ActionedOn,
                    LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
                    TemporaryBlobUrls = lr.LeaveRequestFileNames?
                        .Where(fileName => !string.IsNullOrEmpty(fileName))
                        .Select(fileName => _blobStorageService.GetTemporaryBlobUrl(fileName, _containerNameForLeaveRequestFiles))
                        .ToList()
                });
            }


            //return new ApiResponse(true, "Current and upcoming planned leaves fetched.", 200, currentLeavesToday);
            return new ApiResponse(true, "Current and upcoming planned leaves fetched.", 200, responseDtos);
        }








    }
}
