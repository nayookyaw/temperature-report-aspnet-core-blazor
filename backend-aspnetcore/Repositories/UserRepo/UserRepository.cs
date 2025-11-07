using Microsoft.EntityFrameworkCore;
using BackendAspNetCore.Data;
using BackendAspNetCore.Models;

namespace BackendAspNetCore.Repositories.UserRepo;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetUserById(Guid id)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<List<User>> GetAllUsers() =>
        await _db.Users.AsNoTracking().OrderBy(u => u.Username).ToListAsync();
    
    public async Task<User> CreateUser(User newUser)
    {
        _db.Users.Add(newUser);
        await _db.SaveChangesAsync();
        return newUser;
    }
    public async Task<User> UpdateUser(User updateUser)
    {
        _db.Users.Update(updateUser);
        await _db.SaveChangesAsync();
        return updateUser;
    }
}