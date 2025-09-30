using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RESTful.Api.Context;
using RESTful.Api.Entity;
using RESTful.Api.Service.Implementation;
using RESTful.Api.Service.Interface;

namespace RESTful.UnitTest;

public class UserServiceTests : IDisposable
{
    private readonly BackendDbContext _context;
    private readonly Mock<ICachingService> _mockCachingService;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        // In-Memory Database
        // Create a database which exists only in memory
        var options = new DbContextOptionsBuilder<BackendDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BackendDbContext(options);
        
        // Setups for Mocking
        _mockCachingService = new Mock<ICachingService>();
        _mockLogger = new Mock<ILogger<UserService>>();

        // Service Instance
        _userService = new UserService(_context, _mockCachingService.Object, _mockLogger.Object);

        // Seed test data
        SeedTestData();
    }
    
    private void SeedTestData()
    {
        var users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Age = 25, Position = "Developer", Email = "john@test.com" },
            new User { Id = 2, Name = "Jane Smith", Age = 30, Position = "Manager", Email = "jane@test.com" },
            new User { Id = 3, Name = "Bob Johnson", Age = 35, Position = "Developer", Email = "bob@test.com" },
            new User { Id = 4, Name = "Alice Brown", Age = 28, Position = "Designer", Email = "" }
        };

        _context.Users.AddRange(users);
        _context.SaveChanges();
    }
    
    [Fact]
    public async Task GetUsersByAgeRangeAsync_ValidAgeRange()
    {
        const int minAge = 26;
        const int maxAge = 35;
        
        var key = $"Users_Age_{minAge}_{maxAge}";
        
        _mockCachingService.Setup(x => x.GetAsync<List<User>>(key))
            .ReturnsAsync((List<User>?)null);
        
        var result = await _userService.GetUsersByAgeRangeAsync(minAge, maxAge);
        
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.All(result, user => Assert.True(user.Age >= minAge && user.Age <= maxAge));
    }
    
    [Fact]
    public async Task GetUsersByAgeRangeAsync_InvalidAgeRange()
    {
        const int minAge = 60;
        const int maxAge = 90;
        
        var result = await _userService.GetUsersByAgeRangeAsync(minAge, maxAge);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUsersByPositionAsync_WithMatches()
    {
        const string position = "Developer";
        
        const string key = $"Users_Position_{position}";

        _mockCachingService.Setup(x => x.GetAsync<List<User>>(key))
            .ReturnsAsync((List<User>?)null);
        
        var result = await _userService.GetUsersByPositionAsync(position);
        
        Assert.Equal(2, result.Count);
        Assert.All(result, user => Assert.True(user.Position == position));
    }
    
    [Fact]
    public async Task GetUsersByPositionAsync_InvalidPosition()
    {
        const string position = "NonExistingPosition";
        
        var result = await _userService.GetUsersByPositionAsync(position);
        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetUserByEmailAsync_ValidEmail()
    {
        const string email = "john@test.com";
        
        var key = $"User_Email_{email}";

        _mockCachingService.Setup(x => x.GetAsync<User>(key))
            .ReturnsAsync((User?)null);
        
        var result = await _userService.GetUserByEmailAsync(email);
        
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
    }
    
    [Fact]
    public async Task GetUserByEmailAsync_InvalidEmail()
    {
        const string email = "notexistingemail@test.com";
        
        var result = await _userService.GetUserByEmailAsync(email);
        
        Assert.Null(result);
    }
    
    [Fact]
    public async Task EmailExistsAsync_ValidEmail()
    {
        const string email = "john@test.com";
        
        var result = await _userService.EmailExistsAsync(email);
        
        Assert.True(result);
    }

    
    [Fact]
    public async Task EmailExistsAsync_InvalidEmail()
    {
        const string email = "notexistingemail@test.com";
        
        var result = await _userService.EmailExistsAsync(email);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task EmailExistsAsync_EmptyEmail()
    {
        const string email = "";
        
        var result = await _userService.EmailExistsAsync(email);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task EmailExistsAsync_NullEmail()
    {
        const string email = null;
        
        var result = await _userService.EmailExistsAsync(email);
        
        Assert.False(result);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}