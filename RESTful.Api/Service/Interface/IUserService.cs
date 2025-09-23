using RESTful.Api.Entity;

namespace RESTful.Api.Service.Interface;

public interface IUserService : IGenericService<User>
{
    Task<List<User>> GetUsersByAgeRangeAsync(int minAge, int maxAge);
    Task<List<User>> GetUsersByPositionAsync(string position);
    Task<User?> GetUserByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<List<User>> GetActiveUsersAsync();
}