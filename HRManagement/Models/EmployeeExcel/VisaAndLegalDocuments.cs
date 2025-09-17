using Microsoft.Graph.Models;

namespace HRManagement.Models.EmployeeExcel
{
    public class VisaAndLegalDocuments
    {
        public string EmployeeName { get; set; } = string.Empty;
        public DateOnly? PassportExpiryDate { get; set; }
        public DateOnly? VisaExpiryDate { get; set; }
        public DateOnly? EmiratesIdExpiryDate { get; set; }
        public DateOnly? LabourCardExpiryDate { get; set; }
        public DateOnly? InsuranceExpiryDate { get; set; }
    }
}
