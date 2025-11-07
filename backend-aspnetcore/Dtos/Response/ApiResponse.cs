namespace BackendAspNetCore.Dtos.Response;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
}