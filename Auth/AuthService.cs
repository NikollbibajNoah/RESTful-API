using System.Security.Cryptography;
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
    private readonly ILogger<AuthService> _logger;
    
    public AuthService(AuthDbContext db, IPasswordHasher<AppUser> hasher, IJwtTokenService jwt, ILogger<AuthService> logger)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _logger = logger;
    }
    
    public async Task<AppUser> RegisterAsync(RegisterRequest req)
    {
        // Check for uniqueness
        var exists = await _db.AppUsers
            .AnyAsync(u => u.Username == req.Username || u.Email == req.Email);

        if (exists)
        {
            _logger.LogWarning($"Username {req.Username} or Email {req.Email} already exists");
            throw new ValidationException("Username oder Email ist bereits vergeben.");
        }

        var user = new AppUser
        {
            Username = req.Username,
            Email = req.Email,
            Role = UserRole.User // hard-coded default
        };

        user.PasswordHash = _hasher.HashPassword(user, req.Password);

        _db.AppUsers.Add(user);

        await SaveChangesSafeAsync();
        
        _logger.LogInformation("User {UserId} registered successfully", user.Id);

        return user;
    }
    
    public async Task<JwtTokenResult> LoginAsync(LoginRequest req)
    {
        var user = await _db.AppUsers
            .FirstOrDefaultAsync(u => u.Username == req.UsernameOrEmail || u.Email == req.UsernameOrEmail);

        if (user == null)
        {
            _logger.LogWarning("Login failed for {UsernameOrEmail}", req.UsernameOrEmail);
            throw new ValidationException("Benutzername/Email oder Passwort ist falsch.");
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        
        if (result == PasswordVerificationResult.Failed) 
        {
            _logger.LogWarning("Invalid password for user {UserId}", user.Id);
            throw new ValidationException("Benutzername/Email oder Passwort ist falsch.");
        }

        var tokenResult = _jwt.CreateToken(user);
        
        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            UserId = user.Id
        };
        
        _db.RefreshTokens.Add(refreshToken);
        await SaveChangesSafeAsync();
        
        _logger.LogInformation("User {UserId} logged in successfully", user.Id);
        
        return new JwtTokenResult(
            tokenResult.AccessToken,
            tokenResult.AccessTokenExpiresAtUtc,
            refreshToken.Token,
            refreshToken.Expires
        );
    }
    
    public async Task<JwtTokenResult> RefreshAsync(string refreshTokenValue)
    {
        var refreshToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == refreshTokenValue);

        if (refreshToken == null || refreshToken.IsRevoked || refreshToken.Expires < DateTime.UtcNow)
            throw new ValidationException("Invalid refresh token");

        var user = refreshToken.User ?? throw new ValidationException("User not found");

        var accessTokenResult = _jwt.CreateToken(user);

        // Refresh-Token rotation
        refreshToken.IsRevoked = true;
        var newRefreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            UserId = user.Id
        };

        _db.RefreshTokens.Add(newRefreshToken);
        await SaveChangesSafeAsync();

        return new JwtTokenResult(
            accessTokenResult.AccessToken,
            accessTokenResult.AccessTokenExpiresAtUtc,
            newRefreshToken.Token,
            newRefreshToken.Expires
        );
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