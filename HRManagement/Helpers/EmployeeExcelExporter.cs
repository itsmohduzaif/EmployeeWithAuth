using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using HRManagement.Data;       // Your DbContext namespace
using HRManagement.Models;     // Your Employee model namespace
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using System;
using System.IO;
using System.Linq;

public class EmployeeExcelExporter
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EmployeeExcelExporter(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ExportEmployeesToExcel(string filePath)
    {
        Console.WriteLine("\n\n\n\n\n aaaaaaaaaaaaa\n\n");

        using var scope = _scopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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

        


        int j =2;
        Console.WriteLine($"\n\n\n\n\n {"A"+1} \n\n");
        for (int i=0; i<employees.Count; i++)
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
        var range1 = worksheet1.Range($"A1:P{employees.Count+1}");
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
            //worksheet3.Cell($"B{j}").Value = $"{employees[i].PassportExpiryDate}";
            if (employees[i].PassportExpiryDate.HasValue)
            {
                worksheet3.Cell($"B{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                worksheet3.Cell($"B{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
            }
            else
            {
                //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
                worksheet3.Cell($"B{j}").Value = default(XLCellValue);  // Safe and clean

            }


            //worksheet3.Cell($"C{j}").Value = $"{employees[i].VisaExpiryDate}";
            if (employees[i].PassportExpiryDate.HasValue)
            {
                worksheet3.Cell($"C{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                worksheet3.Cell($"C{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
            }
            else
            {
                //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
                worksheet3.Cell($"C{j}").Value = default(XLCellValue);  // Safe and clean

            }





            //worksheet3.Cell($"D{j}").Value = $"{employees[i].EmiratesIdExpiryDate}";
            if (employees[i].PassportExpiryDate.HasValue)
            {
                worksheet3.Cell($"D{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                worksheet3.Cell($"D{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
            }
            else
            {
                //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
                worksheet3.Cell($"D{j}").Value = default(XLCellValue);  // Safe and clean

            }


            //worksheet3.Cell($"E{j}").Value = $"{employees[i].LabourCardExpiryDate}";
            if (employees[i].PassportExpiryDate.HasValue)
            {
                worksheet3.Cell($"E{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                worksheet3.Cell($"E{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
            }
            else
            {
                //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
                worksheet3.Cell($"E{j}").Value = default(XLCellValue);  // Safe and clean

            }



            //worksheet3.Cell($"F{j}").Value = $"{employees[i].InsuranceExpiryDate}";
            if (employees[i].PassportExpiryDate.HasValue)
            {
                worksheet3.Cell($"F{j}").Value = employees[i].ContractEndDate.Value.ToDateTime(new TimeOnly(0, 0)); // Convert to DateTime
                worksheet3.Cell($"F{j}").Style.NumberFormat.Format = "dd-MMM-yy"; // Custom date format
            }
            else
            {
                //worksheet1.Cell($"E{j}").Value = null; // Empty if no value
                worksheet3.Cell($"F{j}").Value = default(XLCellValue);  // Safe and clean

            }



            j += 1;
        }

        // Creating a table from the data range
        var range3 = worksheet3.Range($"A1:F{employees.Count + 1}");
        var table3 = range3.CreateTable();
        table3.Theme = XLTableTheme.None;
        //Console.WriteLine($"\n\n\n\n\n {"A"+(employees.Count+1)} \n\n");



















        // Save workbook to
        workbook.SaveAs(filePath);


    }
}




//using (var workbook = new XLWorkbook())
//{
//    var worksheet = workbook.Worksheets.Add("Employees");

//    // Create header row
//    var properties = typeof(Employee).GetProperties();
//    for (int i = 0; i < properties.Length; i++)
//    {
//        worksheet.Cell(1, i + 1).Value = properties[i].Name;
//        worksheet.Cell(1, i + 1).Style.Font.Bold = true;
//    }

//    // Fill employee data
//    for (int row = 0; row < employees.Count; row++)
//    {
//        var employee = employees[row];
//        for (int col = 0; col < properties.Length; col++)
//        {
//            var value = properties[col].GetValue(employee);

//            if (value is DateOnly dateOnly)
//                worksheet.Cell(row + 2, col + 1).Value = dateOnly.ToDateTime(TimeOnly.MinValue);
//            else
//                worksheet.Cell(row + 2, col + 1).Value = value?.ToString() ?? "";
//        }
//    }

//    worksheet.Columns().AdjustToContents();

//    workbook.SaveAs(filePath);
//}








//.AsNoTracking()

//This tells Entity Framework not to track the retrieved entities in the change tracker.

//Why use it?

//By default, EF Core tracks the state of entities (to detect changes for updates).

//AsNoTracking() improves performance and reduces memory usage when you only need to read data and don’t plan to modify or save it.

//This is especially useful in reporting/export scenarios like this one.