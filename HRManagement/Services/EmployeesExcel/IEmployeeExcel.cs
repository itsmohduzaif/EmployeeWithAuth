using HRManagement.Models.EmployeeExcel;

namespace HRManagement.Services.EmployeesExcel
{
    public interface IEmployeeExcel
    {

        Task<MemoryStream> ExportEmployeesToExcel();

            
        // // Commented out endpoints for file path based import/export - as we dont have requirement for them.
        //Task ExportEmployeesExcelToFilePath(string filePath);
        //Task ImportExcelData(List<EmployeeBasicDetails> employeeBasicDetailsList,
        //    List<ContactAndAddressDetails> contactAndAddressDetailsList,
        //    List<VisaAndLegalDocuments> visaAndLegalDocumentsList);
        //Task ReadEmployeesFromExcelFromPath(string filePath);





    }
}
