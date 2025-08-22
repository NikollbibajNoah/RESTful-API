using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RESTful.Auth.Interface;
using RESTful.Context;
using RESTful.Entity;
using RESTful.Entity.Auth;
using RESTful.Exceptions;

namespace RESTful.Auth;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _db;
    private readonly IPasswordHasher<AppUser> _hasher;
    private readonly IJwtTokenService _jwt;

    public AuthService(AuthDbContext db, IPasswordHasher<AppUser> hasher, IJwtTokenService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }
    
    public async Task<AppUser> RegisterAsync(RegisterRequest req)
    {
        // Username/Email uniqueness prüfen
        var exists = await _db.AppUsers
            .AnyAsync(u => u.Username == req.Username || u.Email == req.Email);
        if (exists)
            throw new ValidationException("Username oder Email ist bereits vergeben.");

        var user = new AppUser
        {
            Username = req.Username,
            Email = req.Email,
            Role = "User"
        };

        user.PasswordHash = _hasher.HashPassword(user, req.Password);

        _db.AppUsers.Add(user);

        await SaveChangesSafeAsync();

        return user;
    }
    
    public async Task<(string token, DateTime expiresAtUtc)> LoginAsync(LoginRequest req)
    {
        var user = await _db.AppUsers
            .FirstOrDefaultAsync(u => u.Username == req.UsernameOrEmail || u.Email == req.UsernameOrEmail);

        if (user == null)
            throw new ValidationException("Benutzername/Email oder Passwort ist falsch.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        
        if (result == PasswordVerificationResult.Failed)
            throw new ValidationException("Benutzername/Email oder Passwort ist falsch.");

        return _jwt.CreateToken(user);
    }
    
    private async Task SaveChangesSafeAsync()
    {
        try {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) {
            throw new DatabaseException("Failed to save changes.", ex);
        }
    }
}