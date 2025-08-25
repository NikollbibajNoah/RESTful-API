using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using RESTful.Auth.Interface;
using RESTful.Entity.Auth;
using LoginRequest = RESTful.Entity.Auth.LoginRequest;
using RefreshRequest = RESTful.Entity.Auth.RefreshRequest;
using RegisterRequest = RESTful.Entity.Auth.RegisterRequest;

namespace RESTful.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterRequest req)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = await _auth.RegisterAsync(req);
        return CreatedAtAction(nameof(Register), new { id = user.Id }, new { user.Id, user.Username, user.Email });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
    {
        var result = await _auth.LoginAsync(req);
        return Ok(new AuthResponse
        {
            AccessToken = result.AccessToken,
            AccessTokenExpiresAtUtc = result.AccessTokenExpiresAtUtc,
            RefreshToken = result.RefreshToken,
            RefreshTokenExpiresAtUtc = result.RefreshTokenExpiresAtUtc
        });
    }
    
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var result = await _auth.RefreshAsync(request.RefreshToken);
        return Ok(result);
    }
    
    // Test: protected Endpoint
    [Authorize]
    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        return Ok(new
        {
            User = User.Identity?.Name ?? User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value,
            Claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }
}