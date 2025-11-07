using BackendAspNetCore.Models;

namespace BackendAspNetCore.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user) => new UserDto
    {
        Username = user.Username,
        Email = user.Email,
    };
}