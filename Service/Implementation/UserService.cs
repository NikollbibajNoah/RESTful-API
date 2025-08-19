using Microsoft.EntityFrameworkCore;
using RESTful.Context;
using RESTful.Entity;
using RESTful.Exceptions;
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
        var users = await _context.Users.ToListAsync();

        if (users == null || users.Count == 0)
            throw new NotFoundException("No users found in the database.");

        return users;
    }
    
    public async Task<User?> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            throw new NotFoundException($"User with ID {id} was not found.");

        return user;
    }
    
    public async Task<User> CreateUser(User user)
    {
        if (user == null)
            throw new ValidationException("User data cannot be null.");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
    
    public async Task<User?> UpdateUser(int id, User user) 
    {
        if (user == null)
            throw new ValidationException("User data cannot be null.");

        var existing = await _context.Users.FindAsync(id);

        if (existing == null)
            throw new NotFoundException($"User with ID {id} was not found.");

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

        if (user == null)
            throw new NotFoundException($"User with ID {id} was not found.");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return user;
    }
}