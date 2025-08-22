using Microsoft.AspNetCore.Identity.Data;
using RESTful.Entity;
using LoginRequest = RESTful.Entity.Auth.LoginRequest;
using RegisterRequest = RESTful.Entity.Auth.RegisterRequest;

namespace RESTful.Auth.Interface;

public interface IAuthService
{
    Task<AppUser> RegisterAsync(RegisterRequest request);
    Task<(string token, DateTime expiresAtUtc)> LoginAsync(LoginRequest request);
}