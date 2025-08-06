//using HRManagement.Models.Leaves;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace HRManagement.SeedConfiguration
//{
//    public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
//    {
//        public void Configure(EntityTypeBuilder<LeaveBalance> builder)
//        {
//            builder.HasData(
//                // Amit Sharma - Annual, Sick, Compensatory
//                new LeaveBalance { LeaveBalanceId = 1, EmployeeId = 1, LeaveTypeId = 1, TotalAllocated = 20, Used = 5 },
//                new LeaveBalance { LeaveBalanceId = 2, EmployeeId = 1, LeaveTypeId = 2, TotalAllocated = 10, Used = 2 },
//                new LeaveBalance { LeaveBalanceId = 3, EmployeeId = 1, LeaveTypeId = 7, TotalAllocated = 3, Used = 1 },

//                // Nisha Verma - Annual, Sick, Maternity
//                new LeaveBalance { LeaveBalanceId = 4, EmployeeId = 2, LeaveTypeId = 1, TotalAllocated = 20, Used = 10 },
//                new LeaveBalance { LeaveBalanceId = 5, EmployeeId = 2, LeaveTypeId = 2, TotalAllocated = 10, Used = 4 },
//                new LeaveBalance { LeaveBalanceId = 6, EmployeeId = 2, LeaveTypeId = 3, TotalAllocated = 90, Used = 30 },

//                // Rahul Kumar - Sick, Bereavement
//                new LeaveBalance { LeaveBalanceId = 7, EmployeeId = 3, LeaveTypeId = 2, TotalAllocated = 10, Used = 8 },
//                new LeaveBalance { LeaveBalanceId = 8, EmployeeId = 3, LeaveTypeId = 5, TotalAllocated = 5, Used = 2 },

//                // Pooja Mehta - Annual, Paternity (even though not usual), Unpaid
//                new LeaveBalance { LeaveBalanceId = 9, EmployeeId = 4, LeaveTypeId = 1, TotalAllocated = 20, Used = 6 },
//                new LeaveBalance { LeaveBalanceId = 10, EmployeeId = 4, LeaveTypeId = 4, TotalAllocated = 15, Used = 0 },
//                new LeaveBalance { LeaveBalanceId = 11, EmployeeId = 4, LeaveTypeId = 6, TotalAllocated = 0, Used = 2 },

//                // Vikram Singh - Annual, Compensatory, Sick
//                new LeaveBalance { LeaveBalanceId = 12, EmployeeId = 5, LeaveTypeId = 1, TotalAllocated = 20, Used = 7 },
//                new LeaveBalance { LeaveBalanceId = 13, EmployeeId = 5, LeaveTypeId = 7, TotalAllocated = 2, Used = 0 },
//                new LeaveBalance { LeaveBalanceId = 14, EmployeeId = 5, LeaveTypeId = 2, TotalAllocated = 10, Used = 5 }
//            );
//        }
//    }
//}
