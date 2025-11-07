namespace BackendAspNetCore.Dtos.Response;

public class ApiResponseFail : ApiResponse
{
    public static ApiResponseFail FailResponse(string message, int statusCode = 400)
    {
        return new ApiResponseFail()
        {
            StatusCode = statusCode,
            IsSuccess = false,
            Message = message,
        }; 
    }
}