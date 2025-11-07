using BackendAspNetCore.RequestBody.User;

namespace BackendAspNetCore.Services.UserServices;
public interface IUserService
{
    Task<ApiResponse> AddUser(AddUserRequestBody input);
    Task<ApiResponse> UpdateUser(Guid userId, UpdateUserRequestBody input);
    Task<ApiResponse> GetAllUser();
}