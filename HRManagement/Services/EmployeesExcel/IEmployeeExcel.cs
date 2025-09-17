using HRManagement.Models.EmployeeExcel;

namespace HRManagement.Services.EmployeesExcel
{
    public interface IEmployeeExcel
    {
        //void ImportExcelData();

        Task ExportEmployeesToExcel(string filePath);
        Task ImportExcelData(List<EmployeeBasicDetails> employeeBasicDetailsList,
            List<ContactAndAddressDetails> contactAndAddressDetailsList,
            List<VisaAndLegalDocuments> visaAndLegalDocumentsList);
        Task ReadEmployeesFromExcel(string filePath);





    }
}
