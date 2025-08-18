using Microsoft.EntityFrameworkCore;
using RESTful.Context;
using RESTful.Entity;
using RESTful.Service.Interface;

namespace RESTful.Service.Implementation;

public class UserService : IUserService
{
    
    private readonly BackendDBContext _context;

    public UserService(BackendDBContext context)
    {
        _context = context;
    }
    
    public async Task<List<User>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }
    
    public async Task<User?> GetUserById(int id)
    {
        return await _context.FindAsync<User>(id);
    }
    
    public async Task<User> CreateUser(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
    }
    
    public async Task<User?> UpdateUser(int id, User user) 
    {
        var existing = await _context.Users.FindAsync(id);
        
        if (existing == null) return null;

        // Mapping fields
        existing.Name = user.Name;
        existing.Age = user.Age;
        existing.Email = user.Email;
        existing.Position = user.Position;

        await _context.SaveChangesAsync();
        
        return existing;
    }
    
    public async Task<User> DeleteUser(int id) 
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user == null) return null;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        
        return user;
    }
}