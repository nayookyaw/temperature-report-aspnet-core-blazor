
using backend_aspnetcore.Constants;

namespace BackendAspNetCore.Dtos.Response;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    // Factory helpers
    public static ApiResponse<T> SuccessResponse(T data, string message = "Success", int statusCode = StatusCodeConstants.SUCCESS_CODE)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            IsSuccess = true,
            Message = message,
            Data = data,
        };
    }
}