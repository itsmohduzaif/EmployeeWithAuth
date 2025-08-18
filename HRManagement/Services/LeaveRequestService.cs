using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.DTOs.Leaves;
using HRManagement.DTOs.Leaves.LeaveRequest;
using HRManagement.Entities;
using HRManagement.Enums;
using HRManagement.Models.Leaves;
using HRManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HRManagement.JwtFeatures;
using Microsoft.AspNetCore.Identity;
using HRManagement.Models;

namespace HRManagement.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly AppDbContext _context;
        private readonly string _containerNameForLeaveRequestFiles;
        //private readonly BlobStorageServiceForLeaveRequestFiles _blobStorageServiceForLeaveRequestFiles;
        private readonly BlobStorageService _blobStorageService;

        public LeaveRequestService(AppDbContext context, IConfiguration configuration, BlobStorageService blobStorageService)
        {
            _context = context;
            //_blobStorageServiceForLeaveRequestFiles = blobStorageServiceForLeaveRequestFiles;
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


        //// Working fine perfectly
        //// Get all leave requests for one employee
        //public async Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(string usernameFromClaim)
        //{



        //    // Getting the employee from usernameFromClaim 
        //    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
        //    if (employee == null)
        //    {
        //        return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
        //    }



        //    //var leaveRequests = await _context.LeaveRequests.FirstOrDefaultAsync(r => r.EmployeeId == employeeId);
        //    var leaveRequests = await _context.LeaveRequests
        //        .Where(r => r.EmployeeId == employee.EmployeeId)
        //        .OrderByDescending(r => r.RequestedOn)
        //        .ToListAsync();


        //    if (leaveRequests.Count == 0)
        //    {
        //        return new ApiResponse(false, "No leave requests found for this employee.", 404, null);
        //    }



        //    // Loop through each leave request to generate temporary URLs for leave request files
        //    foreach (var leaveRequest in leaveRequests)
        //    {

        //        var leaveRequestUrls = new List<string>();

        //        foreach (var fileName in leaveRequest.LeaveRequestFileNames)
        //        {
        //            if (!string.IsNullOrEmpty(fileName))
        //            {
        //                var tempUrl = _blobStorageServiceForLeaveRequestFiles.GetTemporaryBlobUrl(fileName);
        //                leaveRequestUrls.Add(tempUrl);
        //            }

        //        }

        //        leaveRequest.TemporaryBlobUrls = leaveRequestUrls; 


        //    }




        //    return new ApiResponse(true, "Leave requests fetched successfully.", 200, leaveRequests);

        //}





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
                r.LeaveTypeId == dto.LeaveTypeId &&
                r.Status != LeaveRequestStatus.Rejected &&
                ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
                 (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
                 (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
            );

            if (overlapExists)
                return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);

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
















        // Perfectly working
        //// Employee can create a leave request (with single file upload)  -    Perfectly working
        //public async Task<ApiResponse> CreateLeaveRequestAsync(CreateLeaveRequestDto dto, string usernameFromClaim)
        //{
        //    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
        //    if (employee == null)
        //    {
        //        return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
        //    }

        //    int EmployeeId = employee.EmployeeId;

        //    // Validate dates
        //    if (dto.EndDate < dto.StartDate)
        //        return new ApiResponse(false, "End date can't be before start date.", 400, null);

        //    // Check for overlapping requests for this employee and leave type (Pending or Approved only)
        //    var overlapExists = await _context.LeaveRequests.AnyAsync(r =>
        //        r.EmployeeId == EmployeeId &&
        //        r.LeaveTypeId == dto.LeaveTypeId &&
        //        r.Status != LeaveRequestStatus.Rejected &&
        //        ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
        //         (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
        //         (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
        //    );

        //    if (overlapExists)
        //        return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);

        //    // Validate file upload for leave request
        //    if (dto.file == null || dto.file.Length == 0)
        //        return new ApiResponse(false, "No file provided", 400, null);

        //    // Allowed file extensions for leave request
        //    var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".txt" };
        //    var allowedContentTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/jpg", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" };

        //    var fileExtension = Path.GetExtension(dto.file.FileName).ToLowerInvariant();
        //    if (!allowedExtensions.Contains(fileExtension))
        //    {
        //        return new ApiResponse(false, "Only PDF, JPG, JPEG, PNG, DOCX, DOC, and TXT file formats are allowed.", 400, null);
        //    }

        //    // Check content type
        //    if (!allowedContentTypes.Contains(dto.file.ContentType.ToLower()))
        //    {
        //        return new ApiResponse(false, "Invalid file type. Only PDF, JPG, JPEG, PNG, DOCX, DOC, and TXT formats are allowed.", 400, null);
        //    }

        //    // Check file size (e.g., max 10 MB)
        //    var maxFileSize = 10 * 1024 * 1024; // 10 MB in bytes
        //    if (dto.file.Length > maxFileSize)
        //    {
        //        return new ApiResponse(false, "File size exceeds the maximum allowed size of 10 MB.", 400, null);
        //    }

        //    var uniqueFileName = $"{usernameFromClaim}_{Guid.NewGuid()}{Path.GetExtension(dto.file.FileName)}";

        //    // Upload file to Azure Blob Storage
        //    string blobName = await _blobStorageServiceForLeaveRequestFiles.UploadFileAsync(dto.file, uniqueFileName);

        //    // Create and save the leave request
        //    var leaveRequest = new LeaveRequest
        //    {
        //        EmployeeId = EmployeeId,
        //        LeaveTypeId = dto.LeaveTypeId,
        //        StartDate = dto.StartDate,
        //        EndDate = dto.EndDate,
        //        Reason = dto.Reason,
        //        Status = LeaveRequestStatus.Pending,
        //        ManagerRemarks = null,
        //        RequestedOn = DateTime.UtcNow,
        //        LeaveRequestFileName = blobName // Store the blob name
        //    };

        //    await _context.LeaveRequests.AddAsync(leaveRequest);
        //    await _context.SaveChangesAsync();

        //    return new ApiResponse(true, "Leave request submitted.", 201, leaveRequest);
        //}









        ////Create leave request(validation: overlapping/available balance)
        //public async Task<ApiResponse> CreateLeaveRequestAsync(CreateLeaveRequestDto dto, string usernameFromClaim)
        //{
        //    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
        //    if (employee == null)
        //    {
        //        return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
        //    }

        //    int EmployeeId = employee.EmployeeId;

        //    // Validate dates
        //    if (dto.EndDate < dto.StartDate)
        //        return new ApiResponse(false, "End date can't be before start date.", 400, null);

        //    // Check for overlapping requests for this employee and leave type (Pending or Approved only)
        //    var overlapExists = await _context.LeaveRequests.AnyAsync(r =>
        //        r.EmployeeId == EmployeeId &&
        //        r.LeaveTypeId == dto.LeaveTypeId &&
        //        r.Status != LeaveRequestStatus.Rejected &&
        //        ((dto.StartDate >= r.StartDate && dto.StartDate <= r.EndDate) ||
        //         (dto.EndDate >= r.StartDate && dto.EndDate <= r.EndDate) ||
        //         (dto.StartDate <= r.StartDate && dto.EndDate >= r.EndDate))
        //    );


        //    if (overlapExists)
        //        return new ApiResponse(false, "There is already an overlapping leave request.", 400, null);

        //    ///..............................
        //    //Uploading leave request file to Azure Blob Storage

        //    if (dto.file == null || dto.file.Length == 0)
        //        return new ApiResponse(false, "No file provided", 400, null);


        //    //var uniqueFileName = $"{usernameFromClaim}_{Guid.NewGuid()}{Path.GetExtension(dto.file.FileName)}";

        //    //Console.WriteLine($"\n\n\n\n\n{uniqueFileName}\n\n\n\n");

        //    //return new ApiResponse(true, "Leave request submitted.", 201, dto);


        //    var uniqueFileName = $"{usernameFromClaim}_{Guid.NewGuid()}{Path.GetExtension(dto.file.FileName)}";

        //    string blobName = await _blobStorageServiceForLeaveRequestFiles.UploadFileAsync(dto.file, uniqueFileName);








        //    ///..............................

        //    var leaveRequest = new LeaveRequest
        //    {
        //        EmployeeId = EmployeeId,
        //        LeaveTypeId = dto.LeaveTypeId,
        //        StartDate = dto.StartDate,
        //        EndDate = dto.EndDate,
        //        Reason = dto.Reason,
        //        Status = LeaveRequestStatus.Pending,
        //        ManagerRemarks = null,
        //        RequestedOn = DateTime.UtcNow,
        //        LeaveRequestFileName = blobName // Store the blob name
        //    };

        //    await _context.LeaveRequests.AddAsync(leaveRequest);
        //    await _context.SaveChangesAsync();

        //    return new ApiResponse(true, "Leave request submitted.", 201, leaveRequest);
        //}

        //private async Task<ApiResponse> UploadLeaveRequestFileNameAsync(string usernameFromClaim, IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return new ApiResponse(false, "No file provided", 400, null);


        //    // Validate file extension and content type
        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        //    var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/jpg" };
        //    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        //    Console.WriteLine($"\n\n\n\n\nThe file extension is: {fileExtension}");
        //    if (!allowedExtensions.Contains(fileExtension))
        //    {
        //        return new ApiResponse(false, "Only JPG, JPEG and PNG image formats are allowed", 400, null);
        //    }

        //    Console.WriteLine($"\n\n\n\n\nThe file content type in lowercase is: {file.ContentType.ToLower()}");
        //    if (!allowedContentTypes.Contains(file.ContentType.ToLower()))
        //    {
        //        return new ApiResponse(false, "Only JPG and PNG image formats are allowed", 400, null);
        //    }




        //    var user = await _userManager.FindByNameAsync(usernameFromClaim);
        //    if (user == null)
        //        return new ApiResponse(false, "User not found", 404, null);

        //    var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
        //    if (employee == null)
        //        return new ApiResponse(false, "Employee profile not found", 404, null);

        //    Delete old profile picture blob if exists
        //    if (!string.IsNullOrEmpty(employee.ProfilePictureFileName))
        //        {
        //            try
        //            {
        //                await _blobStorageService.DeleteFileAsync(employee.ProfilePictureFileName);
        //            }
        //            catch (Exception ex)
        //            {
        //                // Log exception, continue without blocking upload
        //                // e.g., _logger.LogWarning($"Failed to delete old profile picture blob: {ex.Message}");
        //                Console.WriteLine(ex.Message);
        //            }
        //        }

        //    var uniqueFileName = $"{usernameFromClaim}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        //    string blobName = await _blobStorageServiceForLeaveRequestFiles.UploadFileAsync(file, uniqueFileName);

        //    employee.ProfilePictureFileName = blobName;
        //    employee.ModifiedBy = usernameFromClaim;
        //    employee.ModifiedDate = DateTime.UtcNow;

        //    await _context.SaveChangesAsync();

        //    return new ApiResponse(true, "Profile picture uploaded successfully", 200, new { PictureBlobName = blobName });
        //}


        // Employee can update their own request if pending
        public async Task<ApiResponse> UpdateLeaveRequestAsync(int requestId, UpdateLeaveRequestDto dto, string usernameFromClaim)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Username == usernameFromClaim);
            if (employee == null)
            {
                return new ApiResponse(false, "Employee not found for the given username for given token.", 404, null);
            }


            var req = await _context.LeaveRequests.FindAsync(requestId);
            if (req == null) {
                return new ApiResponse(false, "Request not found.", 404, null);
            }
            if (req.EmployeeId != employee.EmployeeId) { 
                return new ApiResponse(false, "You can only update your own requests.", 403, null);
            }
            if (req.Status != LeaveRequestStatus.Pending) {
                return new ApiResponse(false, "Can only modify pending requests.", 400, null);
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












        // FOR COPYING CODE     =====------======
        //public async Task<ApiResponse> GetLeaveRequestsForEmployeeAsync(string usernameFromClaim)
        //{
        //    var employee = await _context.Employees
        //        .FirstOrDefaultAsync(e => e.Username == usernameFromClaim);

        //    if (employee == null)
        //    {
        //        return new ApiResponse(false, "Employee not found", 404, null);
        //    }

        //    var leaveRequests = await _context.LeaveRequests
        //        .Where(r => r.EmployeeId == employee.EmployeeId)
        //        .OrderByDescending(r => r.RequestedOn)
        //        .ToListAsync();

        //    if (!leaveRequests.Any())
        //    {
        //        return new ApiResponse(false, "No leave requests found.", 404, null);
        //    }

        //    var responseDtos = leaveRequests.Select(lr => new GetLeaveRequestsForEmployeeDto
        //    {
        //        LeaveRequestId = lr.LeaveRequestId,
        //        EmployeeId = lr.EmployeeId,
        //        LeaveTypeId = lr.LeaveTypeId,
        //        StartDate = lr.StartDate,
        //        EndDate = lr.EndDate,
        //        Reason = lr.Reason,
        //        Status = lr.Status,
        //        ManagerRemarks = lr.ManagerRemarks,
        //        RequestedOn = lr.RequestedOn,
        //        ActionedOn = lr.ActionedOn,
        //        LeaveRequestFileNames = lr.LeaveRequestFileNames ?? new List<string>(),
        //        TemporaryBlobUrls = lr.LeaveRequestFileNames?
        //            .Where(fileName => !string.IsNullOrEmpty(fileName))
        //            .Select(fileName => _blobStorageServiceForLeaveRequestFiles.GetTemporaryBlobUrl(fileName))
        //            .ToList()
        //    }).ToList();

        //    return new ApiResponse(true, "Leave requests fetched successfully.", 200, responseDtos);
        //}





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
    }
}
