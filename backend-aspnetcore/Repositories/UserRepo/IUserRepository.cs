
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Repositories.UserRepo;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid id);
    Task<List<User>> GetAllUsers();
    Task<User?> GetUserByEmail(string email);
    Task<User> CreateUser(User newUser);
    Task<User> UpdateUser(User user);
}