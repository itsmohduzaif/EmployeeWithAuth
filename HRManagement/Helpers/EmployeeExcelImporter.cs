using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using HRManagement.Data;
using HRManagement.DTOs;
using HRManagement.Models;
using HRManagement.Models.EmployeeExcel;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HRManagement.Helpers
{
    public class EmployeeExcelImporter
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EmployeeExcelImporter(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private async void ImportExcelData(List<EmployeeBasicDetails> employeeBasicDetailsList,
            List<ContactAndAddressDetails> contactAndAddressDetailsList,
            List<VisaAndLegalDocuments> visaAndLegalDocumentsList)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();


            var employees = new List<Employee>();

            for (int i = 0; i < employeeBasicDetailsList.Count(); i++)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(e => e.WorkEmail == contactAndAddressDetailsList[i].WorkEmail);

                if (employee == null)
                {
                    // Create a new Employee record and user

                }
                else
                {
                    // Update the existing Emplyee record
                }




            }




        }


        public async Task ReadEmployeesFromExcel(string filePath)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();


            Console.WriteLine("\n\n\n\n\n Reading Excel File \n\n");
            var employeeBasicDetailsList = new List<EmployeeBasicDetails>();
            var contactAndAddressDetailsList = new List<ContactAndAddressDetails>();
            var visaAndLegalDocumentsList = new List<VisaAndLegalDocuments>();


            using (var workbook = new XLWorkbook(filePath))
            {
                Console.WriteLine(" \n\n\n\n\n\nWorkbook loaded");

                Console.WriteLine("📄 Worksheets found in the file:");
                foreach (var sheet in workbook.Worksheets)
                {
                    Console.WriteLine($"- {sheet.Name}");
                }



                var worksheet1 = workbook.Worksheet("Employee Basic details"); // Specify the sheet name
                var rowsOfWorkesheet1 = worksheet1.RowsUsed();


                

                




                foreach (var row in rowsOfWorkesheet1.Skip(1)) // Skip header row
                {
                    Console.WriteLine($"\n\n\n\n\n Reading Row: {row.RowNumber()}      {row.Cell(1).Value.ToString()} \n\n");

                    var EmployeeBasicDetails = new EmployeeBasicDetails
                    {
                        EmployeeName = row.Cell(1).Value.ToString(),
                        Status = row.Cell(2).Value.ToString(),
                        EmploymentType = row.Cell(3).Value.ToString(),
                        ContractBy = row.Cell(4).Value.ToString(),
                        ContractEndDate = row.Cell(5).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(5).GetDateTime()),
                        WorkLocation = row.Cell(6).Value.ToString(),
                        Gender = row.Cell(7).Value.ToString(),
                        Nationality = row.Cell(8).Value.ToString(),
                        DateOfBirth = row.Cell(9).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(9).GetDateTime()),
                        MaritalStatus = row.Cell(10).Value.ToString(),
                        EmiratesIdNumber = row.Cell(11).Value.ToString(),
                        PassportNumber = row.Cell(12).Value.ToString(),
                        JobTitle = row.Cell(13).Value.ToString(),
                        Department = row.Cell(14).Value.ToString(),
                        ManagerName = row.Cell(15).Value.ToString(),
                        DateOfJoining = row.Cell(16).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(16).GetDateTime())
                    };

                    employeeBasicDetailsList.Add(EmployeeBasicDetails);


                } // foreach ends



                // Reading Contact & Address Details sheet
                var worksheet2 = workbook.Worksheet("Contact & Address Details"); // Specify the sheet name
                var rowsOfWorksheet2 = worksheet2.RowsUsed();






                foreach (var row in rowsOfWorksheet2.Skip(1))
                {
                    var contactAndAddressDetails = new ContactAndAddressDetails
                    {
                        EmployeeName = row.Cell(1).Value.ToString(),
                        PersonalEmail = row.Cell(2).Value.ToString(),
                        WorkEmail = row.Cell(3).Value.ToString(),
                        PersonalPhone = row.Cell(4).Value.ToString(),
                        WorkPhone = row.Cell(5).Value.ToString(),
                        EmergencyContactName = row.Cell(6).Value.ToString(),
                        EmergencyContactRelationship = row.Cell(7).Value.ToString(),
                        EmergencyContactNumber = row.Cell(8).Value.ToString(),
                        CurrentAddress = row.Cell(9).Value.ToString(),
                        PermanentAddress = row.Cell(10).Value.ToString(),
                        CountryOfResidence = row.Cell(11).Value.ToString(),
                        PoBox = row.Cell(12).Value.ToString(),
                    };

                    contactAndAddressDetailsList.Add(contactAndAddressDetails);
                }// foreach ends


                
                Console.WriteLine($"\n\n\n\n\n Total Employees Read from Employee Basic Details Sheet: \n\n");
                

                foreach (var emp in contactAndAddressDetailsList)
                { 
                    Console.WriteLine($"\n\n\n\n\n contact and address details      {emp.WorkEmail} \n\n");
                }












                // Reading Worsheet 3: Visa & Legal Documents
                var worksheet3 = workbook.Worksheet("Visa & Legal Documents"); // Specify the sheet name
                var rowsOfWorksheet3 = worksheet3.RowsUsed();






                foreach (var row in rowsOfWorksheet3.Skip(1))
                {
                    var visaAndLegalDocuments = new VisaAndLegalDocuments
                    {
                        EmployeeName = row.Cell(1).Value.ToString(),
                        PassportExpiryDate = row.Cell(2).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(2).GetDateTime()),
                        VisaExpiryDate = row.Cell(3).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(3).GetDateTime()),
                        EmiratesIdExpiryDate = row.Cell(4).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(4).GetDateTime()),
                        LabourCardExpiryDate = row.Cell(5).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(5).GetDateTime()),
                        InsuranceExpiryDate = row.Cell(6).IsEmpty()
                                            ? null
                                            : DateOnly.FromDateTime(row.Cell(6).GetDateTime()),
                    };

                    visaAndLegalDocumentsList.Add(visaAndLegalDocuments);
                }// foreach ends



                Console.WriteLine($"\n\n\n\n\n Total Employees Read from Employee Basic Details Sheet: \n\n");


                foreach (var emp in visaAndLegalDocumentsList)
                {
                    Console.WriteLine($"\n\n\n\n\n contact and address details      {emp.EmiratesIdExpiryDate} \n\n");
                }











            }// using workbook ends

















            //ImportExcelData(employeeBasicDetailsList, contactAndAddressDetailsList, visaAndLegalDocumentsList);

            //// Updating the Employee table in the database
            //foreach (var emp in employeeBasicDetailsList)
            //{
            //    var employee = new Employee
            //    {
            //        EmployeeName = emp.EmployeeName,
            //        Status = emp.Status,
            //        EmploymentType = emp.EmploymentType,
            //        ContractBy = emp.ContractBy,
            //        ContractEndDate = emp.ContractEndDate,
            //        WorkLocation = emp.WorkLocation,

            //    };


            //    await _context.Employees.AddAsync(employee);
            //    await _context.SaveChangesAsync();

            //}


        }
    }
}






    //EmployeeName = row.Cell(1).Value.GetString(),
    //                    Status = row.Cell(2).GetString(),
    //                    EmploymentType = row.Cell(3).GetString(),
    //                    ContractBy = row.Cell(4).GetString(),
    //                    ContractEndDate = row.Cell(5).GetDateTime(),
    //                    WorkLocation = row.Cell(6).GetString(),