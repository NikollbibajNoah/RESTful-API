using Microsoft.AspNetCore.Mvc;
using RESTful.Entity;
using RESTful.Service.Interface;

namespace RESTful.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        this._userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        List<User> users = await _userService.GetAllUsers();

        return Ok(users);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        User foundUser = await _userService.GetUserById(id);
        
        return Ok(foundUser);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        User createdUser = await _userService.CreateUser(user);

        return Ok(createdUser);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(int id, User user)
    {
        User updatedUser = await _userService.UpdateUser(id, user);
        
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<User>> DeleteUser(int id)
    {
        User deletedUser = await _userService.DeleteUser(id);

        return Ok(deletedUser);
    }
}