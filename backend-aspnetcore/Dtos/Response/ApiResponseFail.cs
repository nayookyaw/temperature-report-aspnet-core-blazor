using backend_aspnetcore.Constants;

namespace BackendAspNetCore.Dtos.Response;

public class ApiResponseFail : ApiResponse
{
    public static ApiResponseFail FailResponse(string message, int statusCode = MessageConstants.BAD_REQUEST_CODE)
    {
        return new ApiResponseFail()
        {
            StatusCode = statusCode,
            IsSuccess = false,
            Message = message,
        }; 
    }
}