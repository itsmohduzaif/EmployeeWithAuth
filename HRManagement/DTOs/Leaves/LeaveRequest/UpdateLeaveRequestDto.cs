﻿namespace HRManagement.DTOs.Leaves.LeaveRequest
{
    public class UpdateLeaveRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

}
