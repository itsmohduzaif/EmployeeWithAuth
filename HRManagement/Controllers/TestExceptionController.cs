using Microsoft.AspNetCore.Mvc;
using HRManagement.Exceptions;

[ApiController]
[Route("api/[controller]")]
public class TestExceptionController : ControllerBase
{
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
