using Microsoft.EntityFrameworkCore;
using RESTful.Context;
using RESTful.Entity;
using RESTful.Exceptions;
using RESTful.Service.Interface;

namespace RESTful.Service.Implementation;

public class UserService : IUserService
{
    
    private readonly BackendDBContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(BackendDBContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<List<User>> GetAllUsers()
    {
        _logger.LogInformation("Fetching all users from database...");
        
        var users = await _context.Users.AsNoTracking().ToListAsync();

        if (users.Count == 0)
        {
            _logger.LogWarning("No users found in the database.");
            throw new NotFoundException("No users found in the database.");
        }

        return users;
    }
    
    public async Task<User?> GetUserById(int id)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        
        if (user == null)
            throw new NotFoundException($"User with ID {id} was not found.");

        return user;
    }
    
    public async Task<User> CreateUser(User user)
    {
        if (user == null)
            throw new ValidationException("User data cannot be null.");

        _context.Users.Add(user);
        
        await SaveChangesSafeAsync();

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

        await SaveChangesSafeAsync();

        return existing;
    }
    
    public async Task<User> DeleteUser(int id) 
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            throw new NotFoundException($"User with ID {id} was not found.");

        _context.Users.Remove(user);

        await SaveChangesSafeAsync();

        return user;
    }
    
    private async Task SaveChangesSafeAsync()
    {
        try {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) {
            throw new DatabaseException("Failed to save changes.", ex);
        }
    }
}