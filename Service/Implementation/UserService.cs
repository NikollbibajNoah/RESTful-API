using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RESTful.Common;
using RESTful.Context;
using RESTful.Entity;
using RESTful.Exceptions;
using RESTful.Service.Interface;

namespace RESTful.Service.Implementation;

public class UserService : IUserService
{
    
    private readonly BackendDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UserService> _logger;

    public UserService(BackendDbContext context, IMemoryCache cache, ILogger<UserService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }
    
    public async Task<List<User>> GetAllUsers()
    {
        var key = CacheKeys.UsersAll;
        
        // 1) Try caching
        try
        {
            if (_cache.TryGetValue<List<User>>(key, out var cached))
            {
                _logger.LogInformation($"[CACHE HIT] All users");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache lookup failed for key {CacheKey}", key);
        }
        
        _logger.LogInformation("Fetching all users from database...");

        // 2) Query db
        _logger.LogInformation("[DB QUERY] All users");
        var users = await _context.Users.AsNoTracking().ToListAsync();

        if (users.Count == 0)
        {
            _logger.LogWarning("No users found in the database.");
            throw new NotFoundException("No users found in the database.");
        }
        
        // 3) Put in cache
        var entryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
            .SetSize(5);

        _cache.Set(key, users, entryOptions);

        return users;
    }
    
    public async Task<User?> GetUserById(int id)
    {
        var key = CacheKeys.UserById(id);
        
        // 1) Try caching
        try
        {
            if (_cache.TryGetValue<User>(key, out var cached))
            {
                _logger.LogInformation($"[CACHE HIT] User {id}");
                return cached;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache lookup failed for key {CacheKey}", key);
        }
        
        // 2) Read db
        _logger.LogInformation($"[DB QUERY] User {id}");
        
        var user = await _context.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);
        
        if (user == null)
            throw new NotFoundException($"User with ID {id} was not found.");
        
        // 3) Put in cache
        var entryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
            .SetSize(1);

        _cache.Set(key, user, entryOptions);

        return user;
    }
    
    public async Task<User> CreateUser(User user)
    {
        if (user == null)
            throw new ValidationException("User data cannot be null.");

        _context.Users.Add(user);
        
        await SaveChangesSafeAsync();
        
        var entryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
            .SetSize(1);
        
        _cache.Set(CacheKeys.UserById(user.Id), user, entryOptions);
        
        // Restore cache
        _cache.Remove(CacheKeys.UsersAll);

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
        
        var updated = await _context.Users.AsNoTracking()
            .FirstAsync(u => u.Id == id);
        
        var entryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
            .SetSlidingExpiration(TimeSpan.FromSeconds(30))
            .SetSize(1);
        
        _cache.Set(CacheKeys.UserById(id), updated, entryOptions);
        _cache.Remove(CacheKeys.UsersAll); // Optional

        return updated;
    }
    
    public async Task<User?> DeleteUser(int id) 
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
            throw new NotFoundException($"User with ID {id} was not found.");

        _context.Users.Remove(user);

        await SaveChangesSafeAsync();
        
        _cache.Remove(CacheKeys.UserById(id)); // Remove from cache
        _cache.Remove(CacheKeys.UsersAll); // Restore cache

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