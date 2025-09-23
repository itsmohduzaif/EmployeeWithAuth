using HRManagement.Models.Leaves;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.SeedConfiguration
{
    public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
    {
        public void Configure(EntityTypeBuilder<LeaveType> builder)
        {
            builder.HasData(
                new LeaveType
                {
                    LeaveTypeId = 1,
                    LeaveTypeName = "Annual Leave",
                    LeaveTypeDescription = "Paid leave for vacation or personal reasons.",
                    DefaultAnnualAllocation = 22
                },
                new LeaveType
                {
                    LeaveTypeId = 2,
                    LeaveTypeName = "Sick Leave",
                    LeaveTypeDescription = "Leave granted for health-related issues.",
                    DefaultAnnualAllocation = 90
                },
                new LeaveType
                {
                    LeaveTypeId = 3,
                    LeaveTypeName = "Personal/Casual Leave",
                    LeaveTypeDescription = "Leave for personal matters",
                    DefaultAnnualAllocation = 0
                },
                new LeaveType
                {
                    LeaveTypeId = 4,
                    LeaveTypeName = "Emergency Leave",
                    LeaveTypeDescription = "Leave if there is any emergency.",
                    DefaultAnnualAllocation = 0
                }
            );
        }
    }
}
