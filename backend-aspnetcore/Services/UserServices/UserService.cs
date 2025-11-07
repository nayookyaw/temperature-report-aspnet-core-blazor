using BackendAspNetCore.Dtos.Response;
using BackendAspNetCore.Models;
using BackendAspNetCore.Mappers;
using BackendAspNetCore.Repositories.UserRepo;
using BackendAspNetCore.Utils;
using BackendAspNetCore.RequestBody.User;

namespace BackendAspNetCore.Services.UserServices;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;
    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<ApiResponse> AddUser(AddUserRequestBody input)
    {
        User? existUser = await _userRepo.GetUserByEmail(input.Email);
        if (existUser != null)
        {
            return ApiResponseFail.FailResponse("Email is already exist,", 400);
        }
        var newUser = new User
        {
            Username = input.Username.Trim(),
            Email = input.Email.Trim(),
            Password = TextUtil.GenerateSecureRandomString(12),
        };
        User user = await _userRepo.CreateUser(newUser);

        UserDto userDto = UserMapper.ToDto(user);
        return ApiResponse<UserDto>.SuccessResponse(userDto, "New user has been added", 200);
    }

    public async Task<ApiResponse> UpdateUser(Guid userId, UpdateUserRequestBody input)
    {
        User? existUser = await _userRepo.GetUserById(userId);
        if (existUser == null)
        {
            return ApiResponseFail.FailResponse("No user is found!", 400);
        }
        existUser.Email = input.Email;
        await _userRepo.UpdateUser(existUser);
        UserDto userDto = UserMapper.ToDto(existUser);
        return ApiResponse<UserDto>.SuccessResponse(userDto, "User has been updated", 200);
    }
    public async Task<ApiResponse> GetAllUser()
    {
        var userList = await _userRepo.GetAllUsers();
        List<UserDto> userDtoList = userList.Select(u => UserMapper.ToDto(u)).ToList();

        if (userDtoList.Count == 0)
        {
            return ApiResponseFail.FailResponse("No user is found", 400);
        }

        return ApiResponse<List<UserDto>>.SuccessResponse(userDtoList, "Users has been retrieved", 200);
    }
}