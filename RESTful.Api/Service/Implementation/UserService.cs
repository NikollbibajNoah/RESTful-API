using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RESTful.Api.Context;
using RESTful.Api.Entity;
using RESTful.Api.Service.Interface;

namespace RESTful.Api.Service.Implementation;

public class UserService : GenericService<User>, IUserService
{
    private readonly BackendDbContext _context;
    private readonly ICachingService _cachingService;
    private readonly ILogger<UserService> _logger;
    
    public UserService(BackendDbContext context, ICachingService cachingService, ILogger<UserService> logger) 
        : base(context, cachingService, logger)
    {
        _context = context;
        _cachingService = cachingService;
        _logger = logger;
    }
    
    public async Task<List<User>> GetUsersByAgeRangeAsync(int minAge, int maxAge)
    {
        var key = $"Users_Age_{minAge}_{maxAge}";

        var cached = await _cachingService.GetAsync<List<User>>(key);
        if (cached != null)
            return cached;

        _logger.LogInformation("[DB QUERY] Users by age range {MinAge}-{MaxAge}", minAge, maxAge);
        var users = await _context.Users
            .Where(u => u.Age >= minAge && u.Age <= maxAge)
            .AsNoTracking()
            .ToListAsync();

        // Apply caching
        await _cachingService.SetAsync(key, users);
        
        return users;
    }

    public async Task<List<User>> GetUsersByPositionAsync(string position)
    {
        var key = $"Users_Position_{position}";

        var cached = await _cachingService.GetAsync<List<User>>(key);
        if (cached != null)
            return cached;

        _logger.LogInformation("[DB QUERY] Users by position {Position}", position);
        var users = await _context.Users
            .Where(u => u.Position.ToLower() == position.ToLower())
            .AsNoTracking()
            .ToListAsync();

        // Apply caching
        await _cachingService.SetAsync(key, users);
        
        return users;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var key = $"User_Email_{email}";

        var cached = await _cachingService.GetAsync<User>(key);
        if (cached != null)
            return cached;

        _logger.LogInformation("[DB QUERY] User by email {Email}", email);
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        // Apply caching if user is found
        if (user != null)
        {
            await _cachingService.SetAsync(key, user);
        }

        return user;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
            return false;
        
        return await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<List<User>> GetActiveUsersAsync()
    {
        const string key = "Users_Active";

        var cached = await _cachingService.GetAsync<List<User>>(key);
        if (cached != null)
            return cached;

        _logger.LogInformation("[DB QUERY] Active users");
        var users = await _context.Users
            .Where(u => !string.IsNullOrEmpty(u.Email))
            .AsNoTracking()
            .ToListAsync();

        // Apply caching
        await _cachingService.SetAsync(key, users);
        
        return users;
    }
}