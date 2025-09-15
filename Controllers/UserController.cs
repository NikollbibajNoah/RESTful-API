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

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var foundUser = await _userService.GetUserById(id);

        return Ok(foundUser);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        var createdUser = await _userService.CreateUser(user);

        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser); // 201
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> UpdateUser(int id, User user)
    {
        var updatedUser = await _userService.UpdateUser(id, user);

        return Ok(updatedUser);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<User>> DeleteUser(int id)
    {
        var deletedUser = await _userService.DeleteUser(id);

        return Ok(deletedUser);
    }
}