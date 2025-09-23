using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RESTful.Api.Entity;
using RESTful.Api.Entity.Auth;
using RESTful.Api.Service.Interface;
using RESTful.Api.Exceptions;
using RESTful.Api.Service.Implementation;

namespace RESTful.Api.Controllers;

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
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var foundUser = await _userService.GetByIdAsync(id);

        return Ok(foundUser);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        var createdUser = await _userService.CreateAsync(user);

        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser); // 201
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> UpdateUser(int id, User user)
    {
        var updatedUser = await _userService.UpdateAsync(id, user);

        return Ok(updatedUser);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> DeleteUser(int id)
    {
        var deletedUser = await _userService.DeleteAsync(id);

        return Ok(deletedUser);
    }
    
    [HttpGet("age-range")]
    [Authorize]
    public async Task<ActionResult<List<User>>> GetUsersByAgeRange(
        [FromQuery] int minAge = 0,
        [FromQuery] int maxAge = 150)
    {
        var users = await _userService.GetUsersByAgeRangeAsync(minAge, maxAge);
        return Ok(users);
    }

    [HttpGet("position/{position}")]
    [Authorize]
    public async Task<ActionResult<List<User>>> GetUsersByPosition(string position)
    {
        var users = await _userService.GetUsersByPositionAsync(position);
        return Ok(users);
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound();
        return Ok(user);
    }
}