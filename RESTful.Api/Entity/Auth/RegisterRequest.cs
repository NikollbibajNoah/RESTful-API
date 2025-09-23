using System.ComponentModel.DataAnnotations;

namespace RESTful.Api.Entity.Auth;

public class RegisterRequest
{
    [Required, MaxLength(64)]
    public string Username { get; set; } = null!;

    [Required, MaxLength(128), EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;
}