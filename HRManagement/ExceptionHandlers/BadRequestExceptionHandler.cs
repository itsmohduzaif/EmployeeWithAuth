using HRManagement.DTOs;
using HRManagement.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace HRManagement.ExceptionHandlers
{
    internal sealed class BadRequestExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<BadRequestExceptionHandler> _logger;

        public BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not BadRequestException badRequestException)
            {
                return false;
            }

            _logger.LogError(
                badRequestException,
                "Exception occurred: {Message}",
                badRequestException.Message);

            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                Message = badRequestException.Message,
                StatusCode = StatusCodes.Status400BadRequest,
                Response = null
            };

            httpContext.Response.StatusCode = apiResponse.StatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);


            return true;
        }
    }
}
