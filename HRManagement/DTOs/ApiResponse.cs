namespace HRManagement.DTOs
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public object? Response { get; set; }

        public ApiResponse() { }

        public ApiResponse(bool isSuccess, string message, int statusCode, object? response)
        {
            IsSuccess = isSuccess;
            Message = message;
            StatusCode = statusCode;
            Response = response;
        }
    }
}
