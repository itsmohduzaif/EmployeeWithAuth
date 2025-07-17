using HRManagement.DTOs;
using HRManagement.Exceptions;
using Microsoft.AspNetCore.Diagnostics;


namespace HRManagement.ExceptionHandlers
{
    internal sealed class NotFoundExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<NotFoundExceptionHandler> _logger;

        public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not NotFoundException notFoundException)
            {
                return false;
            }

            _logger.LogError(
                notFoundException,
                "Exception occurred: {Message}",
                notFoundException.Message);

            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                Message = notFoundException.Message,
                StatusCode = StatusCodes.Status404NotFound,
                Response = null
            };

            httpContext.Response.StatusCode = apiResponse.StatusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(apiResponse, cancellationToken);

            return true;
        }
    }
}
