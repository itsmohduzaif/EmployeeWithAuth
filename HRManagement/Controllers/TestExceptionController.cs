
// Will Delete this Controller

using HRManagement.Exceptions;
using HRManagement.Helpers;
using HRManagement.Services.EmployeesExcel;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TestExceptionController : ControllerBase
{
    private readonly ILogger<TestExceptionController> _logger;
    private readonly EmployeeExcelExporter _employeeExcelExporter;
    private readonly EmployeeExcelImporter _employeeExcelImporter;
    private readonly IEmployeeExcel _employeeExcel;

    //Microsoft Identity Client

    public TestExceptionController(ILogger<TestExceptionController> logger, EmployeeExcelExporter employeeExcelExporter, EmployeeExcelImporter employeeExcelImporter, IEmployeeExcel employeeExcel)
    {
        _logger = logger;
        _employeeExcelExporter = employeeExcelExporter;
        _employeeExcelImporter = employeeExcelImporter;
        _employeeExcel = employeeExcel;

    }

    // created this endpoint just for debugging purpose, will delete later
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Console.WriteLine("\n\n\n\n\n hehehehn\n\n");

        //Commenting the export code to test import code
        //_employeeExcelExporter.ExportEmployeesToExcel(@"C:\Users\user\Downloads\test7.xlsx");


        //_employeeExcelImporter.ReadEmployeesFromExcel(@"C:\Users\user\Downloads\test7.xlsx");



        // Calling from scoped

        await _employeeExcel.ExportEmployeesToExcel(@"C:\Users\user\Downloads\test7.xlsx");

        //await _employeeExcel.ReadEmployeesFromExcel(@"C:\Users\user\Downloads\test7.xlsx");



        return Ok("Excel file exported successfully.");
    }


    //// created this endpoint just for debugging purpose, will delete later
    //[HttpGet("check-leave-days")]
    //public async Task<IActionResult> CheckLeaveDays()
    //{
    //    DateTime startDate = new DateTime(2025, 9, 22, 15, 30, 0);
    //    DateTime endDate = new DateTime(2025, 9, 24, 13, 1, 0);
    //    decimal effectiveLeaveDays = CalculateEffectiveLeaveDays.GetEffectiveLeaveDays(startDate, endDate);
    //    return Ok($"Endpoint Executed, the value of effectiveLeaveDays is: {effectiveLeaveDays}");
    //}


    [HttpGet("bad-request")]
    public IActionResult ThrowBadRequest()
    {
        throw new BadRequestException("This is a bad request test exception.");
    }

    [HttpGet("not-found")]
    public IActionResult ThrowNotFound()
    {
        throw new NotFoundException("This is a not found test exception.");
    }

    [HttpGet("server-error")]
    public IActionResult ThrowServerError()
    {
        throw new Exception("This is a generic server error test exception.");
    }
}
