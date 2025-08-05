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
                    DefaultAnnualAllocation = 20
                },
                new LeaveType
                {
                    LeaveTypeId = 2,
                    LeaveTypeName = "Sick Leave",
                    LeaveTypeDescription = "Leave granted for health-related issues.",
                    DefaultAnnualAllocation = 10
                },
                new LeaveType
                {
                    LeaveTypeId = 3,
                    LeaveTypeName = "Maternity Leave",
                    LeaveTypeDescription = "Leave for childbirth and recovery.",
                    DefaultAnnualAllocation = 90
                },
                new LeaveType
                {
                    LeaveTypeId = 4,
                    LeaveTypeName = "Paternity Leave",
                    LeaveTypeDescription = "Leave for fathers during childbirth period.",
                    DefaultAnnualAllocation = 15
                },
                new LeaveType
                {
                    LeaveTypeId = 5,
                    LeaveTypeName = "Bereavement Leave",
                    LeaveTypeDescription = "Leave in case of death of a family member.",
                    DefaultAnnualAllocation = 5
                },
                new LeaveType
                {
                    LeaveTypeId = 6,
                    LeaveTypeName = "Unpaid Leave",
                    LeaveTypeDescription = "Leave without salary deduction for personal matters.",
                    DefaultAnnualAllocation = 0
                },
                new LeaveType
                {
                    LeaveTypeId = 7,
                    LeaveTypeName = "Compensatory Leave",
                    LeaveTypeDescription = "Leave earned by working extra hours or holidays.",
                    DefaultAnnualAllocation = 0
                }
            );
        }
    }
}
