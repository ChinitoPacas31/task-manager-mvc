using MongoDB.Driver;
using TaskManager.API.Data;
using TaskManager.API.Models;

namespace TaskManager.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MongoDbContext _context;

    public UserRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        return await _context.Users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.Find(_ => true).ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.InsertOneAsync(user);
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        var result = await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _context.Users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await _context.Users.Find(u => u.Username == username).AnyAsync();
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.Find(u => u.Email == email).AnyAsync();
    }
}
