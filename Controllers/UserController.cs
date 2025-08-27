using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTful.Entity;
using RESTful.Entity.Auth;
using RESTful.Exceptions;
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
    [Authorize]
    public async Task<ActionResult<List<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsers();
        // if (users == null || users.Count == 0) return NoContent();

        return Ok(users);
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var foundUser = await _userService.GetUserById(id);

        // if (foundUser == null) throw new NotFoundException($"User with ID {id} was not found.");
        
        return Ok(foundUser);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        // if (!ModelState.IsValid) throw new ValidationException("User model is invalid.");

        var createdUser = await _userService.CreateUser(user);

        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser); // 201
    }

    [HttpPut("{id}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> UpdateUser(int id, User user)
    {
        // if (id != user.Id) throw new ValidationException($"Given ID {id} does not match User ID {user.Id}.");
        //
        // if (!ModelState.IsValid) throw new ValidationException("User model is invalid.");
        
        var updatedUser = await _userService.UpdateUser(id, user);

        // if (updatedUser == null) return NotFound();
        
        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> DeleteUser(int id)
    {
        var deletedUser = await _userService.DeleteUser(id);
        
        // if (deletedUser == null) throw new NotFoundException($"User with ID {id} was not found.");

        return Ok(deletedUser);
    }
}