using System.ComponentModel.DataAnnotations;
using RESTful.Entity.Auth;

namespace RESTful.Entity;

public class AppUser
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Username { get; set; } = null!;

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required, MaxLength(32)]
    public UserRole Role { get; set; } = UserRole.User;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}