using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RESTful.Api.Context;
using RESTful.Api.Entity;
using RESTful.Api.Service.Implementation;
using RESTful.Api.Service.Interface;

namespace RESTful.UnitTest;

public class GenericServiceTests : IDisposable
{
    private readonly BackendDbContext _context;
    private readonly Mock<ICachingService> _mockCachingService;
    private readonly Mock<ILogger<GenericService<User>>> _mockLogger;
    private readonly GenericService<User> _genericService;

    public GenericServiceTests()
    {
        // In-Memory Database
        // Create a database which exists only in memory
        var options = new DbContextOptionsBuilder<BackendDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BackendDbContext(options);

        // Setups for Mocking
        _mockCachingService = new Mock<ICachingService>();
        _mockLogger = new Mock<ILogger<GenericService<User>>>();

        // Service Instance
        _genericService = new GenericService<User>(_context, _mockCachingService.Object, _mockLogger.Object);

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
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        _mockCachingService.Setup(x => x.GetAsync<List<User>>("Users_all"))
            .ReturnsAsync((List<User>?)null);

        var result = await _genericService.GetAllAsync();

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectUser()
    {
        _mockCachingService.Setup(x => x.GetAsync<User>("User_1"))
            .ReturnsAsync((User?)null);

        var result = await _genericService.GetByIdAsync(1);

        Assert.Equal("John Doe", result.Name);
    }

    [Fact]
    public async Task CreateAsync_AddsNewUser()
    {
        var newUser = new User
            { Name = "Charlie White", Age = 29, Position = "Tester", Email = "charliewhite@example.com" };
        
        var result = await _genericService.CreateAsync(newUser);
        
        Assert.True(result.Id > 0);
        Assert.Equal("Charlie White", result.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingUser()
    {
        var updatedUser = new User
            { Id = 1, Name = "Updated Charlie White", Age = 35, Position = "CEO", Email = "charliewhite@example.com" };
        
        var result = await _genericService.UpdateAsync(1, updatedUser);
        
        Assert.Equal("Updated Charlie White", result.Name);
        Assert.Equal(35, result.Age);
        Assert.Equal("CEO", result.Position);
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        var result = await _genericService.DeleteAsync(1);
        
        Assert.Equal("John Doe", result.Name);
        var deletedUser = await _context.Users.FindAsync(1);
        Assert.Null(deletedUser);
    }

    [Fact]
    public void Dispose()
    {
        _context.Dispose();
    }
}