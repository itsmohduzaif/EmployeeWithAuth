using System.ComponentModel.DataAnnotations;

namespace HRManagement.DTOs.EmployeeDTOs
{
    public class EmployeeCreateDTO
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? UserName { get; set; }
        //[Required(ErrorMessage = "Email is required.")]
        //public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; } = null;
        [Required(ErrorMessage = "Role is required.")]
        public string? EmployeeRole { get; set; }
        public string EmployeeName { get; set; } = string.Empty; // new field
        //public string FirstName { get; set; } = string.Empty;
        //public string LastName { get; set; } = string.Empty;
        //public string PhoneNumber { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; } = string.Empty;


        // New Properties from Excel file
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
        [Required(ErrorMessage = "Work Email is required.")]
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