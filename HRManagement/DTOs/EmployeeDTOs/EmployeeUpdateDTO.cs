namespace HRManagement.DTOs.EmployeeDTOs
{
    public class EmployeeUpdateDTO
    {
        public int EmployeeId { get; set; }  // Required to identify the employee
        //public string Email { get; set; } = string.Empty;
        //public string FirstName { get; set; } = string.Empty;
        //public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        //public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;


        // New Properties from Excel file
        public string EmployeeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string ContractBy { get; set; } = string.Empty;
        public DateOnly? ContractEndDate { get; set; }
        public string WorkLocation { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
        public string EmiratesIdNumber { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public DateOnly? DateOfJoining { get; set; }

        // Contact & Address
        public string PersonalEmail { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string PersonalPhone { get; set; } = string.Empty;
        public string WorkPhone { get; set; } = string.Empty;
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactRelationship { get; set; } = string.Empty;
        public string EmergencyContactNumber { get; set; } = string.Empty;
        public string CurrentAddress { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string CountryOfResidence { get; set; } = string.Empty;
        public string PoBox { get; set; } = string.Empty;

        // Visa & Legal Documents
        public DateOnly? PassportExpiryDate { get; set; }
        public DateOnly? VisaExpiryDate { get; set; }
        public DateOnly? EmiratesIdExpiryDate { get; set; }
        public DateOnly? LabourCardExpiryDate { get; set; }
        public DateOnly? InsuranceExpiryDate { get; set; }
    }
}
