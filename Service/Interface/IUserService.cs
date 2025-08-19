using RESTful.Entity;

namespace RESTful.Service.Interface;

public interface IUserService
{

    /// <summary>
    /// Retrieves all existing users from database
    /// </summary>
    /// <returns>List of users</returns>
    public Task<List<User>> GetAllUsers();
    
    
    /// <summary>
    /// Retrieves single found user by individual identifier
    /// </summary>
    /// <param name="id">Individual identifier</param>
    /// <returns>Found user by given id</returns>
    public Task<User?> GetUserById(int id);
    
    /// <summary>
    /// Adds new user into database
    /// </summary>
    /// <param name="user">Newly created user</param>
    /// <returns>Created and saved user from database</returns>
    public Task<User> CreateUser(User user);
    
    /// <summary>
    /// Update existing user from database by given individual identifier
    /// </summary>
    /// <param name="id">Individual identifier</param>
    /// <param name="user">New user data to overwrite old</param>
    /// <returns>Newly updated and saved user from database</returns>
    public Task<User?> UpdateUser(int id, User user);
    
    /// <summary>
    /// Removes single user from database by given individual identifier
    /// </summary>
    /// <param name="id">Individual identifier</param>
    /// <returns>Deleted user from database</returns>
    public Task<User?> DeleteUser(int id);
}