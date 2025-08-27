namespace HRManagement.DTOs.EmployeeDTOs
{
    public class EmployeeProfileDTO
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string EmployeeRole { get; set; } = string.Empty;
        public string? ProfilePictureFileName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }



        // New Properties from Excel file
        public string EmployeeFullName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string ContractBy { get; set; } = string.Empty;
        public DateTime? ContractEndDate { get; set; }
        public string WorkLocation { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string MaritalStatus { get; set; } = string.Empty;
        public string EmiratesIdNumber { get; set; } = string.Empty;
        public string PassportNumber { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public DateTime? DateOfJoining { get; set; }

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
        public DateTime? PassportExpiryDate { get; set; }
        public DateTime? VisaExpiryDate { get; set; }
        public DateTime? EmiratesIdExpiryDate { get; set; }
        public DateTime? LabourCardExpiryDate { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }

    }

}
