using HRManagement.DTOs;
using Microsoft.AspNetCore.Diagnostics;

namespace HRManagement.ExceptionHandlers
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception, "Exception occurred: {Message}", exception.Message);

            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                Message = "An unexpected error occurred.",
                StatusCode = StatusCodes.Status500InternalServerError,
                Response = null
            };

            httpContext.Response.StatusCode = apiResponse.StatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);


            return true;
        }
    }
}
