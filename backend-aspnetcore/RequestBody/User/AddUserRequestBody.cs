namespace BackendAspNetCore.RequestBody.User;

public class AddUserRequestBody
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}