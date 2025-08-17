using RESTful.Entity;
using RESTful.Service.Interface;

namespace RESTful.Service.Implementation;

public class UserService : IUserService
{

    public async Task<List<User>> GetAllUsers()
    {

        return null;
    }
    
    public Task<User> GetUserById(int id)
    {
        return null;
    }
    
    public Task<User> CreateUser(User user) 
    {
        return null;
    }
    
    public Task<User> UpdateUser(int id, User user) 
    {
        return null;
    }
    
    public Task<User> DeleteUser(int id) 
    {
        return null;
    }
}