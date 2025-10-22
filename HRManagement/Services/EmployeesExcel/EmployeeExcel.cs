//.....
using Microsoft.Identity.Client;
using System.Net.Http;

//.....
using ClosedXML.Excel;
using HRManagement.Data;
using HRManagement.DTOs.EmployeeDTOs;
using HRManagement.Models;
using HRManagement.Models.EmployeeExcel;
using HRManagement.Services.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace HRManagement.Services.EmployeesExcel
{
    public class EmployeeExcel : IEmployeeExcel
    {
        private readonly AppDbContext _context;
        private readonly IEmployeeService _employeeService;

        public EmployeeExcel(AppDbContext context, IEmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }


        public async Task<MemoryStream> ExportEmployeesToExcel()
        {
            Console.WriteLine("\n\n\n\n\n aaaaaaaaaaaaa\n\n");


            var employees = await _context.Employees.AsNoTracking().ToListAsync();


            using var workbook = new XLWorkbook();

            var worksheet1 = workbook.Worksheets.Add("Employee Basic details");

            // Styling
            worksheet1.Row(1).Style.Font.Bold = true;

            // Adding headers
            worksheet1.Cell("A1").Value = "Employee Name";
            worksheet1.Cell("B1").Value = "Status";
            worksheet1.Cell("C1").Value = "Employment Type";
            worksheet1.Cell("D1").Value = "Contract By";
            worksheet1.Cell("E1").Value = "Contract End Date";
            worksheet1.Cell("F1").Value = "Work Location";
            worksheet1.Cell("G1").Value = "Gender";
            worksheet1.Cell("H1").Value = "Nationality";
            worksheet1.Cell("I1").Value = "Date of Birth";
            worksheet1.Cell("J1").Value = "Marital Status";
            worksheet1.Cell("K1").Value = "Emirates ID Number";
            worksheet1.Cell("L1").Value = "Passport Number";
            worksheet1.Cell("M1").Value = "Job Title";
            worksheet1.Cell("N1").Value = "Department";
            worksheet1.Cell("O1").Value = "Manager Name";
            worksheet1.Cell("P1").Value = "Date of Joining";




            int j = 2;
            Console.WriteLine($"\n\n\n\n\n {"A" + 1} \n\n");
            for (int i = 0; i < employees.Count; i++)
            {

                worksheet1.Cell($"A{j}").Value = $"{employees[i].EmployeeName}";
                worksheet1.Cell($"B{j}").Value = $"{employees[i].Status}";
                worksheet1.Cell($"C{j}").Value = $"{employees[i].EmploymentType}";
                worksheet1.Cell($"D{j}").Value = $"{employees[i].ContractBy}";


                // Convert ContractEndDate (DateOnly) to DateTime and apply custom format
                if (employees[i].ContractEndDate.HasValue)
                {
                    worksheet1.Cell($"E{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                    worksheet1.Cell($"E{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
                }
                else
                {
                    //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
                    worksheet1.Cell($"E{j}").Value = default(XLCellValue);  // Safe and clean

                }

                worksheet1.Cell($"F{j}").Value = $"{employees[i].WorkLocation}";
                worksheet1.Cell($"G{j}").Value = $"{employees[i].Gender}";
                worksheet1.Cell($"H{j}").Value = $"{employees[i].Nationality}";


                // Convert DateOfBirth (DateOnly) to DateTime and apply custom format
                if (employees[i].DateOfBirth.HasValue)
                {
                    worksheet1.Cell($"I{j}").Value = employees[i].DateOfBirth.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                    worksheet1.Cell($"I{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
                }
                else
                {
                    //worksheet1.Cell($"I{j}").Value = string.Empty; // Empty if no value
                    worksheet1.Cell($"I{j}").Value = default(XLCellValue);  // Safe and clean
                }

                worksheet1.Cell($"J{j}").Value = $"{employees[i].MaritalStatus}";
                worksheet1.Cell($"K{j}").Value = $"{employees[i].EmiratesIdNumber}";
                worksheet1.Cell($"L{j}").Value = $"{employees[i].PassportNumber}";
                worksheet1.Cell($"M{j}").Value = $"{employees[i].JobTitle}";
                worksheet1.Cell($"N{j}").Value = $"{employees[i].Department}";
                worksheet1.Cell($"O{j}").Value = $"{employees[i].ManagerName}";


                // Convert DateOfJoining (DateOnly) to DateTime and apply custom format
                if (employees[i].DateOfJoining.HasValue)
                {
                    worksheet1.Cell($"P{j}").Value = employees[i].DateOfJoining.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                    worksheet1.Cell($"P{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
                }
                else
                {
                    //worksheet1.Cell($"P{j}").Value = string.Empty; // Empty if no value
                    worksheet1.Cell($"P{j}").Value = default(XLCellValue);  // Safe and clean
                }

                j += 1;




                ////Styling
                //var currentRow = worksheet1.Row(i+1);

                //// Add border to entire row
                //currentRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                //currentRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                //// Apply light gray background to even rows (for striping)
                //if (i % 2 == 0)
                //{
                //    currentRow.Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242); // Light gray
                //}


            }


            // Creating a table from the data range
            var range1 = worksheet1.Range($"A1:P{employees.Count + 1}");
            var table1 = range1.CreateTable();
            table1.Theme = XLTableTheme.None;
            table1.ShowAutoFilter = false;  // This disables the dropdowns
                                            //Console.WriteLine($"\n\n\n\n\n {"A"+(employees.Count+1)} \n\n");









            // Creating sheet 2 for Contact & Address Details
            var worksheet2 = workbook.Worksheets.Add("Contact & Address Details");

            // Styling
            worksheet2.Row(1).Style.Font.Bold = true;

            // Adding headers
            worksheet2.Cell("A1").Value = "Employee Name";
            worksheet2.Cell("B1").Value = "Personal Email";
            worksheet2.Cell("C1").Value = "Work Email";
            worksheet2.Cell("D1").Value = "Personal Phone";

            worksheet2.Cell("E1").Value = "Work Phone";
            worksheet2.Cell("F1").Value = "Emergency Contact Name";
            worksheet2.Cell("G1").Value = "Emergency Contact Relationship";
            worksheet2.Cell("H1").Value = "Emergency Contact Number";
            worksheet2.Cell("I1").Value = "Current Address";
            worksheet2.Cell("J1").Value = "Permanent Address";
            worksheet2.Cell("K1").Value = "Country of Residence";
            worksheet2.Cell("L1").Value = "PO Box";


            j = 2;
            Console.WriteLine($"\n\n\n\n\n {"A" + 1} \n\n");
            for (int i = 0; i < employees.Count; i++)
            {

                worksheet2.Cell($"A{j}").Value = $"{employees[i].EmployeeName}";
                worksheet2.Cell($"B{j}").Value = $"{employees[i].PersonalEmail}";
                worksheet2.Cell($"C{j}").Value = $"{employees[i].WorkEmail}";
                worksheet2.Cell($"D{j}").Value = $"{employees[i].PersonalPhone}";

                worksheet2.Cell($"E{j}").Value = $"{employees[i].WorkPhone}";
                worksheet2.Cell($"F{j}").Value = $"{employees[i].EmergencyContactName}";
                worksheet2.Cell($"G{j}").Value = $"{employees[i].EmergencyContactRelationship}";
                worksheet2.Cell($"H{j}").Value = $"{employees[i].EmergencyContactNumber}";
                worksheet2.Cell($"I{j}").Value = $"{employees[i].CurrentAddress}";


                worksheet2.Cell($"J{j}").Value = $"{employees[i].PermanentAddress}";
                worksheet2.Cell($"K{j}").Value = $"{employees[i].CountryOfResidence}";
                worksheet2.Cell($"L{j}").Value = $"{employees[i].PoBox}";

                j += 1;
            }

            // Creating a table from the data range
            var range2 = worksheet2.Range($"A1:L{employees.Count + 1}");
            var table2 = range2.CreateTable();
            table2.Theme = XLTableTheme.None;
            //Console.WriteLine($"\n\n\n\n\n {"A"+(employees.Count+1)} \n\n");















            // Creating sheet 3 for Visa & Legal Documents
            var worksheet3 = workbook.Worksheets.Add("Visa & Legal Documents");

            // Styling
            worksheet3.Row(1).Style.Font.Bold = true;

            // Adding headers
            worksheet3.Cell("A1").Value = "Employee Name";
            worksheet3.Cell("B1").Value = "Passport Expiry Date";
            worksheet3.Cell("C1").Value = "Visa Expiry Date";
            worksheet3.Cell("D1").Value = "Emirates ID Expiry Date";
            worksheet3.Cell("E1").Value = "Labour Card Expiry Date";
            worksheet3.Cell("F1").Value = "Insurance Expiry Date";


            j = 2;
            Console.WriteLine($"\n\n\n\n\n {"A" + 1} \n\n");
            

            for (int i = 0; i < employees.Count; i++)
            {
                worksheet3.Cell($"A{j}").Value = $"{employees[i].EmployeeName}";

                if (employees[i].PassportExpiryDate.HasValue)
                {
                    worksheet3.Cell($"B{j}").Value = employees[i].PassportExpiryDate.Value.ToDateTime(new TimeOnly(0, 0));
                    worksheet3.Cell($"B{j}").Style.NumberFormat.Format = "dd-MMM-yy";
                }
                else
                {
                    worksheet3.Cell($"B{j}").Value = default(XLCellValue);
                }

                if (employees[i].VisaExpiryDate.HasValue)
                {
                    worksheet3.Cell($"C{j}").Value = employees[i].VisaExpiryDate.Value.ToDateTime(new TimeOnly(0, 0));
                    worksheet3.Cell($"C{j}").Style.NumberFormat.Format = "dd-MMM-yy";
                }
                else
                {
                    worksheet3.Cell($"C{j}").Value = default(XLCellValue);
                }

                if (employees[i].EmiratesIdExpiryDate.HasValue)
                {
                    worksheet3.Cell($"D{j}").Value = employees[i].EmiratesIdExpiryDate.Value.ToDateTime(new TimeOnly(0, 0));
                    worksheet3.Cell($"D{j}").Style.NumberFormat.Format = "dd-MMM-yy";
                }
                else
                {
                    worksheet3.Cell($"D{j}").Value = default(XLCellValue);
                }

                if (employees[i].LabourCardExpiryDate.HasValue)
                {
                    worksheet3.Cell($"E{j}").Value = employees[i].LabourCardExpiryDate.Value.ToDateTime(new TimeOnly(0, 0));
                    worksheet3.Cell($"E{j}").Style.NumberFormat.Format = "dd-MMM-yy";
                }
                else
                {
                    worksheet3.Cell($"E{j}").Value = default(XLCellValue);
                }

                if (employees[i].InsuranceExpiryDate.HasValue)
                {
                    worksheet3.Cell($"F{j}").Value = employees[i].InsuranceExpiryDate.Value.ToDateTime(new TimeOnly(0, 0));
                    worksheet3.Cell($"F{j}").Style.NumberFormat.Format = "dd-MMM-yy";
                }
                else
                {
                    worksheet3.Cell($"F{j}").Value = default(XLCellValue);
                }

                j++;
            }



            // Creating a table from the data range
            var range3 = worksheet3.Range($"A1:F{employees.Count + 1}");
            var table3 = range3.CreateTable();
            table3.Theme = XLTableTheme.None;
            //Console.WriteLine($"\n\n\n\n\n {"A"+(employees.Count+1)} \n\n");







            // Save workbook to local path
            //workbook.SaveAs(filePath);   // Commenting as we dont want to save in the local directory


            // Instead of lcal path, now lets save to memory stream
            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;


            return stream;









        }

















        //  Below endpoints are not for any use for now./  maybe if needed in future.
        // so no unit testing will be done for them.


        // But since we currently dont have any requirement for them, so i am commenting them out.

        //public async Task ExportEmployeesExcelToFilePath(string filePath)
        //{
        //    Console.WriteLine("\n\n\n\n\n aaaaaaaaaaaaa\n\n");


        //    var employees = await _context.Employees.AsNoTracking().ToListAsync();


        //    using var workbook = new XLWorkbook();

        //    var worksheet1 = workbook.Worksheets.Add("Employee Basic details");

        //    // Styling
        //    worksheet1.Row(1).Style.Font.Bold = true;

        //    // Adding headers
        //    worksheet1.Cell("A1").Value = "Employee Name";
        //    worksheet1.Cell("B1").Value = "Status";
        //    worksheet1.Cell("C1").Value = "Employment Type";
        //    worksheet1.Cell("D1").Value = "Contract By";
        //    worksheet1.Cell("E1").Value = "Contract End Date";
        //    worksheet1.Cell("F1").Value = "Work Location";
        //    worksheet1.Cell("G1").Value = "Gender";
        //    worksheet1.Cell("H1").Value = "Nationality";
        //    worksheet1.Cell("I1").Value = "Date of Birth";
        //    worksheet1.Cell("J1").Value = "Marital Status";
        //    worksheet1.Cell("K1").Value = "Emirates ID Number";
        //    worksheet1.Cell("L1").Value = "Passport Number";
        //    worksheet1.Cell("M1").Value = "Job Title";
        //    worksheet1.Cell("N1").Value = "Department";
        //    worksheet1.Cell("O1").Value = "Manager Name";
        //    worksheet1.Cell("P1").Value = "Date of Joining";




        //    int j = 2;
        //    Console.WriteLine($"\n\n\n\n\n {"A" + 1} \n\n");
        //    for (int i = 0; i < employees.Count; i++)
        //    {

        //        worksheet1.Cell($"A{j}").Value = $"{employees[i].EmployeeName}";
        //        worksheet1.Cell($"B{j}").Value = $"{employees[i].Status}";
        //        worksheet1.Cell($"C{j}").Value = $"{employees[i].EmploymentType}";
        //        worksheet1.Cell($"D{j}").Value = $"{employees[i].ContractBy}";


        //        // Convert ContractEndDate (DateOnly) to DateTime and apply custom format
        //        if (employees[i].ContractEndDate.HasValue)
        //        {
        //            worksheet1.Cell($"E{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet1.Cell($"E{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
        //            worksheet1.Cell($"E{j}").Value = default(XLCellValue);  // Safe and clean

        //        }

        //        worksheet1.Cell($"F{j}").Value = $"{employees[i].WorkLocation}";
        //        worksheet1.Cell($"G{j}").Value = $"{employees[i].Gender}";
        //        worksheet1.Cell($"H{j}").Value = $"{employees[i].Nationality}";


        //        // Convert DateOfBirth (DateOnly) to DateTime and apply custom format
        //        if (employees[i].DateOfBirth.HasValue)
        //        {
        //            worksheet1.Cell($"I{j}").Value = employees[i].DateOfBirth.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet1.Cell($"I{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"I{j}").Value = string.Empty; // Empty if no value
        //            worksheet1.Cell($"I{j}").Value = default(XLCellValue);  // Safe and clean
        //        }

        //        worksheet1.Cell($"J{j}").Value = $"{employees[i].MaritalStatus}";
        //        worksheet1.Cell($"K{j}").Value = $"{employees[i].EmiratesIdNumber}";
        //        worksheet1.Cell($"L{j}").Value = $"{employees[i].PassportNumber}";
        //        worksheet1.Cell($"M{j}").Value = $"{employees[i].JobTitle}";
        //        worksheet1.Cell($"N{j}").Value = $"{employees[i].Department}";
        //        worksheet1.Cell($"O{j}").Value = $"{employees[i].ManagerName}";


        //        // Convert DateOfJoining (DateOnly) to DateTime and apply custom format
        //        if (employees[i].DateOfJoining.HasValue)
        //        {
        //            worksheet1.Cell($"P{j}").Value = employees[i].DateOfJoining.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet1.Cell($"P{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"P{j}").Value = string.Empty; // Empty if no value
        //            worksheet1.Cell($"P{j}").Value = default(XLCellValue);  // Safe and clean
        //        }

        //        j += 1;




        //        ////Styling
        //        //var currentRow = worksheet1.Row(i+1);

        //        //// Add border to entire row
        //        //currentRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        //        //currentRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        //        //// Apply light gray background to even rows (for striping)
        //        //if (i % 2 == 0)
        //        //{
        //        //    currentRow.Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242); // Light gray
        //        //}


        //    }


        //    // Creating a table from the data range
        //    var range1 = worksheet1.Range($"A1:P{employees.Count + 1}");
        //    var table1 = range1.CreateTable();
        //    table1.Theme = XLTableTheme.None;
        //    table1.ShowAutoFilter = false;  // This disables the dropdowns
        //                                    //Console.WriteLine($"\n\n\n\n\n {"A"+(employees.Count+1)} \n\n");









        //    // Creating sheet 2 for Contact & Address Details
        //    var worksheet2 = workbook.Worksheets.Add("Contact & Address Details");

        //    // Styling
        //    worksheet2.Row(1).Style.Font.Bold = true;

        //    // Adding headers
        //    worksheet2.Cell("A1").Value = "Employee Name";
        //    worksheet2.Cell("B1").Value = "Personal Email";
        //    worksheet2.Cell("C1").Value = "Work Email";
        //    worksheet2.Cell("D1").Value = "Personal Phone";

        //    worksheet2.Cell("E1").Value = "Work Phone";
        //    worksheet2.Cell("F1").Value = "Emergency Contact Name";
        //    worksheet2.Cell("G1").Value = "Emergency Contact Relationship";
        //    worksheet2.Cell("H1").Value = "Emergency Contact Number";
        //    worksheet2.Cell("I1").Value = "Current Address";
        //    worksheet2.Cell("J1").Value = "Permanent Address";
        //    worksheet2.Cell("K1").Value = "Country of Residence";
        //    worksheet2.Cell("L1").Value = "PO Box";


        //    j = 2;
        //    Console.WriteLine($"\n\n\n\n\n {"A" + 1} \n\n");
        //    for (int i = 0; i < employees.Count; i++)
        //    {

        //        worksheet2.Cell($"A{j}").Value = $"{employees[i].EmployeeName}";
        //        worksheet2.Cell($"B{j}").Value = $"{employees[i].PersonalEmail}";
        //        worksheet2.Cell($"C{j}").Value = $"{employees[i].WorkEmail}";
        //        worksheet2.Cell($"D{j}").Value = $"{employees[i].PersonalPhone}";

        //        worksheet2.Cell($"E{j}").Value = $"{employees[i].WorkPhone}";
        //        worksheet2.Cell($"F{j}").Value = $"{employees[i].EmergencyContactName}";
        //        worksheet2.Cell($"G{j}").Value = $"{employees[i].EmergencyContactRelationship}";
        //        worksheet2.Cell($"H{j}").Value = $"{employees[i].EmergencyContactNumber}";
        //        worksheet2.Cell($"I{j}").Value = $"{employees[i].CurrentAddress}";


        //        worksheet2.Cell($"J{j}").Value = $"{employees[i].PermanentAddress}";
        //        worksheet2.Cell($"K{j}").Value = $"{employees[i].CountryOfResidence}";
        //        worksheet2.Cell($"L{j}").Value = $"{employees[i].PoBox}";

        //        j += 1;
        //    }

        //    // Creating a table from the data range
        //    var range2 = worksheet2.Range($"A1:L{employees.Count + 1}");
        //    var table2 = range2.CreateTable();
        //    table2.Theme = XLTableTheme.None;
        //    //Console.WriteLine($"\n\n\n\n\n {"A"+(employees.Count+1)} \n\n");















        //    // Creating sheet 3 for Visa & Legal Documents
        //    var worksheet3 = workbook.Worksheets.Add("Visa & Legal Documents");

        //    // Styling
        //    worksheet3.Row(1).Style.Font.Bold = true;

        //    // Adding headers
        //    worksheet3.Cell("A1").Value = "Employee Name";
        //    worksheet3.Cell("B1").Value = "Passport Expiry Date";
        //    worksheet3.Cell("C1").Value = "Visa Expiry Date";
        //    worksheet3.Cell("D1").Value = "Emirates ID Expiry Date";
        //    worksheet3.Cell("E1").Value = "Labour Card Expiry Date";
        //    worksheet3.Cell("F1").Value = "Insurance Expiry Date";


        //    j = 2;
        //    Console.WriteLine($"\n\n\n\n\n {"A" + 1} \n\n");
        //    for (int i = 0; i < employees.Count; i++)
        //    {

        //        worksheet3.Cell($"A{j}").Value = $"{employees[i].EmployeeName}";
        //        //worksheet3.Cell($"B{j}").Value = $"{employees[i].PassportExpiryDate}";
        //        if (employees[i].PassportExpiryDate.HasValue)
        //        {
        //            worksheet3.Cell($"B{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet3.Cell($"B{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
        //            worksheet3.Cell($"B{j}").Value = default(XLCellValue);  // Safe and clean

        //        }


        //        //worksheet3.Cell($"C{j}").Value = $"{employees[i].VisaExpiryDate}";
        //        if (employees[i].PassportExpiryDate.HasValue)
        //        {
        //            worksheet3.Cell($"C{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet3.Cell($"C{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
        //            worksheet3.Cell($"C{j}").Value = default(XLCellValue);  // Safe and clean

        //        }





        //        //worksheet3.Cell($"D{j}").Value = $"{employees[i].EmiratesIdExpiryDate}";
        //        if (employees[i].PassportExpiryDate.HasValue)
        //        {
        //            worksheet3.Cell($"D{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet3.Cell($"D{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
        //            worksheet3.Cell($"D{j}").Value = default(XLCellValue);  // Safe and clean

        //        }


        //        //worksheet3.Cell($"E{j}").Value = $"{employees[i].LabourCardExpiryDate}";
        //        if (employees[i].PassportExpiryDate.HasValue)
        //        {
        //            worksheet3.Cell($"E{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet3.Cell($"E{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
        //            worksheet3.Cell($"E{j}").Value = default(XLCellValue);  // Safe and clean

        //        }



        //        //worksheet3.Cell($"F{j}").Value = $"{employees[i].InsuranceExpiryDate}";
        //        if (employees[i].PassportExpiryDate.HasValue)
        //        {
        //            worksheet3.Cell($"F{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
        //            worksheet3.Cell($"F{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
        //        }
        //        else
        //        {
        //            //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
        //            worksheet3.Cell($"F{j}").Value = default(XLCellValue);  // Safe and clean

        //        }



        //        j += 1;
        //    }

        //    // Creating a table from the data range
        //    var range3 = worksheet3.Range($"A1:F{employees.Count + 1}");
        //    var table3 = range3.CreateTable();
        //    table3.Theme = XLTableTheme.None;
        //    //Console.WriteLine($"\n\n\n\n\n {"A"+(employees.Count+1)} \n\n");







        //    // Save workbook to local path
        //    workbook.SaveAs(filePath);   // Commenting as we dont want to save in the local directory












        //}


























        // Since we currently dont have any requirement for the endpoint below, so i am commenting out this also.





        //public async Task ImportExcelData(List<EmployeeBasicDetails> employeeBasicDetailsList,
        //    List<ContactAndAddressDetails> contactAndAddressDetailsList,
        //    List<VisaAndLegalDocuments> visaAndLegalDocumentsList)
        //{
        //    //Console.WriteLine("\n\n\n\n\n Importing Excel Data \n\n");
        //    //var employee = await _context.Employees.FirstOrDefaultAsync(e => e.WorkEmail == contactAndAddressDetailsList[1].WorkEmail);
        //    //Console.WriteLine($"\n\n\n\n\n Employee fetched from DB: {employee?.EmployeeName} - {employee?.WorkEmail} \n\n");


        //    var employees = new List<Employee>();

        //    for (int i = 0; i < employeeBasicDetailsList.Count(); i++)
        //    {


        //        if (employeeBasicDetailsList[i].EmployeeName != contactAndAddressDetailsList[i].EmployeeName || contactAndAddressDetailsList[i].EmployeeName != visaAndLegalDocumentsList[i].EmployeeName || employeeBasicDetailsList[i].EmployeeName != visaAndLegalDocumentsList[i].EmployeeName)
        //        {
        //            throw new Exception("The EmployeeName in different sheets of the excel file is in-consistent.");
        //        }


        //        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.WorkEmail == contactAndAddressDetailsList[i].WorkEmail);

        //        if (employee == null)
        //        {
        //            // Create a new Employee record and user

        //            // Step 1: Create EmployeeCreateDTO

        //            var employeeDto = new EmployeeCreateDTO
        //            {
        //                UserName = contactAndAddressDetailsList[i].WorkEmail,
        //                Password = "Initial@12354", // Default password, consider changing it later
        //                EmployeeRole = "Employee", // Default role, can be changed later
        //                EmployeeName = employeeBasicDetailsList[i].EmployeeName,
        //                IsActive = true,
        //                CreatedBy = "System", // or fetch from context if available

        //                // New Properties from Excel file
        //                Status = employeeBasicDetailsList[i].Status,
        //                EmploymentType = employeeBasicDetailsList[i].EmploymentType,
        //                ContractBy = employeeBasicDetailsList[i].ContractBy,
        //                ContractEndDate = employeeBasicDetailsList[i].ContractEndDate,
        //                WorkLocation = employeeBasicDetailsList[i].WorkLocation,
        //                Gender = employeeBasicDetailsList[i].Gender,
        //                Nationality = employeeBasicDetailsList[i].Nationality,
        //                DateOfBirth = employeeBasicDetailsList[i].DateOfBirth,
        //                MaritalStatus = employeeBasicDetailsList[i].MaritalStatus,
        //                EmiratesIdNumber = employeeBasicDetailsList[i].EmiratesIdNumber,
        //                PassportNumber = employeeBasicDetailsList[i].PassportNumber,
        //                JobTitle = employeeBasicDetailsList[i].JobTitle,
        //                Department = employeeBasicDetailsList[i].Department,
        //                ManagerName = employeeBasicDetailsList[i].ManagerName,
        //                DateOfJoining = employeeBasicDetailsList[i].DateOfJoining,

        //                // Contact & Address
        //                PersonalEmail = contactAndAddressDetailsList[i].PersonalEmail,
        //                WorkEmail = contactAndAddressDetailsList[i].WorkEmail,
        //                PersonalPhone = contactAndAddressDetailsList[i].PersonalPhone,
        //                WorkPhone = contactAndAddressDetailsList[i].WorkPhone,
        //                EmergencyContactName = contactAndAddressDetailsList[i].EmergencyContactName,
        //                EmergencyContactRelationship = contactAndAddressDetailsList[i].EmergencyContactRelationship,
        //                EmergencyContactNumber = contactAndAddressDetailsList[i].EmergencyContactNumber,
        //                CurrentAddress = contactAndAddressDetailsList[i].CurrentAddress,
        //                PermanentAddress = contactAndAddressDetailsList[i].PermanentAddress,
        //                CountryOfResidence = contactAndAddressDetailsList[i].CountryOfResidence,
        //                PoBox = contactAndAddressDetailsList[i].PoBox,

        //                // Visa & Legal Documents
        //                PassportExpiryDate = visaAndLegalDocumentsList[i].PassportExpiryDate,
        //                VisaExpiryDate = visaAndLegalDocumentsList[i].VisaExpiryDate,
        //                EmiratesIdExpiryDate = visaAndLegalDocumentsList[i].EmiratesIdExpiryDate,
        //                LabourCardExpiryDate = visaAndLegalDocumentsList[i].LabourCardExpiryDate,
        //                InsuranceExpiryDate = visaAndLegalDocumentsList[i].InsuranceExpiryDate

        //            };

        //            // Step 2: Call the CreateEmployee method from IEmployeeService
        //            var result = await _employeeService.CreateEmployee(employeeDto);



        //        }
        //        else
        //        {
        //            // Update the existing Emplyee record

        //            // Step 1: Create EmployeeUpdateDTO
        //            var updated = new EmployeeUpdateDTO
        //            {
        //                EmployeeId = employee.EmployeeId,
        //                UserName = employee.UserName,
        //                IsActive = employee.IsActive,
        //                ModifiedBy = employee.ModifiedBy,

        //                // New Properties from Excel file
        //                EmployeeName = employeeBasicDetailsList[i].EmployeeName,
        //                Status = employeeBasicDetailsList[i].Status,
        //                EmploymentType = employeeBasicDetailsList[i].EmploymentType,
        //                ContractBy = employeeBasicDetailsList[i].ContractBy,
        //                ContractEndDate = employeeBasicDetailsList[i].ContractEndDate,
        //                WorkLocation = employeeBasicDetailsList[i].WorkLocation,
        //                Gender = employeeBasicDetailsList[i].Gender,
        //                Nationality = employeeBasicDetailsList[i].Nationality,
        //                DateOfBirth = employeeBasicDetailsList[i].DateOfBirth,
        //                MaritalStatus = employeeBasicDetailsList[i].MaritalStatus,
        //                EmiratesIdNumber = employeeBasicDetailsList[i].EmiratesIdNumber,
        //                PassportNumber = employeeBasicDetailsList[i].PassportNumber,
        //                JobTitle = employeeBasicDetailsList[i].JobTitle,
        //                Department = employeeBasicDetailsList[i].Department,
        //                ManagerName = employeeBasicDetailsList[i].ManagerName,
        //                DateOfJoining = employeeBasicDetailsList[i].DateOfJoining,

        //                // Contact & Address
        //                PersonalEmail = contactAndAddressDetailsList[i].PersonalEmail,
        //                WorkEmail = contactAndAddressDetailsList[i].WorkEmail,
        //                PersonalPhone = contactAndAddressDetailsList[i].PersonalPhone,
        //                WorkPhone = contactAndAddressDetailsList[i].WorkPhone,
        //                EmergencyContactName = contactAndAddressDetailsList[i].EmergencyContactName,
        //                EmergencyContactRelationship = contactAndAddressDetailsList[i].EmergencyContactRelationship,
        //                EmergencyContactNumber = contactAndAddressDetailsList[i].EmergencyContactNumber,
        //                CurrentAddress = contactAndAddressDetailsList[i].CurrentAddress,
        //                PermanentAddress = contactAndAddressDetailsList[i].PermanentAddress,
        //                CountryOfResidence = contactAndAddressDetailsList[i].CountryOfResidence,
        //                PoBox = contactAndAddressDetailsList[i].PoBox,

        //                // Visa & Legal Documents
        //                PassportExpiryDate = visaAndLegalDocumentsList[i].PassportExpiryDate,
        //                VisaExpiryDate = visaAndLegalDocumentsList[i].VisaExpiryDate,
        //                EmiratesIdExpiryDate = visaAndLegalDocumentsList[i].EmiratesIdExpiryDate,
        //                LabourCardExpiryDate = visaAndLegalDocumentsList[i].LabourCardExpiryDate,
        //                InsuranceExpiryDate = visaAndLegalDocumentsList[i].InsuranceExpiryDate



        //            };




        //            // Step 2: Call UpdateEmployee
        //            var result = await _employeeService.UpdateEmployee(updated);

        //        }




        //    }




        //}



        // Since we currently dont have any requirement for the endpoint below, so i am commenting out this as well.

        //public async Task ReadEmployeesFromExcelFromPath(string filePath)
        //{
        //    Console.WriteLine("\n\n\n\n\n Reading Excel File \n\n");
        //    var employeeBasicDetailsList = new List<EmployeeBasicDetails>();
        //    var contactAndAddressDetailsList = new List<ContactAndAddressDetails>();
        //    var visaAndLegalDocumentsList = new List<VisaAndLegalDocuments>();


        //    using (var workbook = new XLWorkbook(filePath))
        //    {
        //        Console.WriteLine(" \n\n\n\n\n\nWorkbook loaded");

        //        Console.WriteLine("📄 Worksheets found in the file:");
        //        foreach (var sheet in workbook.Worksheets)
        //        {
        //            Console.WriteLine($"- {sheet.Name}");
        //        }



        //        var worksheet1 = workbook.Worksheet("Employee Basic details"); // Specify the sheet name
        //        var rowsOfWorkesheet1 = worksheet1.RowsUsed();









        //        foreach (var row in rowsOfWorkesheet1.Skip(1)) // Skip header row
        //        {
        //            Console.WriteLine($"\n\n\n\n\n Reading Row: {row.RowNumber()}      {row.Cell(1).Value.ToString()} \n\n");

        //            var EmployeeBasicDetails = new EmployeeBasicDetails
        //            {
        //                EmployeeName = row.Cell(1).Value.ToString(),
        //                Status = row.Cell(2).Value.ToString(),
        //                EmploymentType = row.Cell(3).Value.ToString(),
        //                ContractBy = row.Cell(4).Value.ToString(),
        //                ContractEndDate = row.Cell(5).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(5).GetDateTime()),
        //                WorkLocation = row.Cell(6).Value.ToString(),
        //                Gender = row.Cell(7).Value.ToString(),
        //                Nationality = row.Cell(8).Value.ToString(),
        //                DateOfBirth = row.Cell(9).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(9).GetDateTime()),
        //                MaritalStatus = row.Cell(10).Value.ToString(),
        //                EmiratesIdNumber = row.Cell(11).Value.ToString(),
        //                PassportNumber = row.Cell(12).Value.ToString(),
        //                JobTitle = row.Cell(13).Value.ToString(),
        //                Department = row.Cell(14).Value.ToString(),
        //                ManagerName = row.Cell(15).Value.ToString(),
        //                DateOfJoining = row.Cell(16).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(16).GetDateTime())
        //            };

        //            employeeBasicDetailsList.Add(EmployeeBasicDetails);


        //        } // foreach ends



        //        // Reading Contact & Address Details sheet
        //        var worksheet2 = workbook.Worksheet("Contact & Address Details"); // Specify the sheet name
        //        var rowsOfWorksheet2 = worksheet2.RowsUsed();






        //        foreach (var row in rowsOfWorksheet2.Skip(1))
        //        {
        //            var contactAndAddressDetails = new ContactAndAddressDetails
        //            {
        //                EmployeeName = row.Cell(1).Value.ToString(),
        //                PersonalEmail = row.Cell(2).Value.ToString(),
        //                WorkEmail = row.Cell(3).Value.ToString(),
        //                PersonalPhone = row.Cell(4).Value.ToString(),
        //                WorkPhone = row.Cell(5).Value.ToString(),
        //                EmergencyContactName = row.Cell(6).Value.ToString(),
        //                EmergencyContactRelationship = row.Cell(7).Value.ToString(),
        //                EmergencyContactNumber = row.Cell(8).Value.ToString(),
        //                CurrentAddress = row.Cell(9).Value.ToString(),
        //                PermanentAddress = row.Cell(10).Value.ToString(),
        //                CountryOfResidence = row.Cell(11).Value.ToString(),
        //                PoBox = row.Cell(12).Value.ToString(),
        //            };

        //            contactAndAddressDetailsList.Add(contactAndAddressDetails);
        //        }// foreach ends



        //        Console.WriteLine($"\n\n\n\n\n Total Employees Read from Employee Basic Details Sheet: \n\n");


        //        foreach (var emp in contactAndAddressDetailsList)
        //        {
        //            Console.WriteLine($"\n\n\n\n\n contact and address details      {emp.WorkEmail} \n\n");
        //        }












        //        // Reading Worsheet 3: Visa & Legal Documents
        //        var worksheet3 = workbook.Worksheet("Visa & Legal Documents"); // Specify the sheet name
        //        var rowsOfWorksheet3 = worksheet3.RowsUsed();






        //        foreach (var row in rowsOfWorksheet3.Skip(1))
        //        {
        //            var visaAndLegalDocuments = new VisaAndLegalDocuments
        //            {
        //                EmployeeName = row.Cell(1).Value.ToString(),
        //                PassportExpiryDate = row.Cell(2).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(2).GetDateTime()),
        //                VisaExpiryDate = row.Cell(3).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(3).GetDateTime()),
        //                EmiratesIdExpiryDate = row.Cell(4).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(4).GetDateTime()),
        //                LabourCardExpiryDate = row.Cell(5).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(5).GetDateTime()),
        //                InsuranceExpiryDate = row.Cell(6).IsEmpty()
        //                                    ? null
        //                                    : DateOnly.FromDateTime(row.Cell(6).GetDateTime()),
        //            };

        //            visaAndLegalDocumentsList.Add(visaAndLegalDocuments);
        //        }// foreach ends



        //        Console.WriteLine($"\n\n\n\n\n Total Employees Read from Employee Basic Details Sheet: \n\n");


        //        foreach (var emp in visaAndLegalDocumentsList)
        //        {
        //            Console.WriteLine($"\n\n\n\n\n contact and address details      {emp.EmiratesIdExpiryDate} \n\n");
        //        }











        //    }// using workbook ends




        //    await ImportExcelData(employeeBasicDetailsList, contactAndAddressDetailsList, visaAndLegalDocumentsList);
        //}
    }
}
