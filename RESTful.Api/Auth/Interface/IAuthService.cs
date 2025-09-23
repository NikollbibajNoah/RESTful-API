using Microsoft.AspNetCore.Identity.Data;
using RESTful.Api.Entity;
using RESTful.Api.Entity.Auth;
using Auth_LoginRequest = RESTful.Api.Entity.Auth.LoginRequest;
using Auth_RegisterRequest = RESTful.Api.Entity.Auth.RegisterRequest;
using LoginRequest = RESTful.Api.Entity.Auth.LoginRequest;
using RegisterRequest = RESTful.Api.Entity.Auth.RegisterRequest;

namespace RESTful.Api.Auth.Interface;

public interface IAuthService
{
    /// <summary>
    /// Register a new user by given register request which consists of unique name,
    /// email-adress and password. After successfully validation, registered user saved in database.
    /// </summary>
    /// <param name="request">Register request: username, email and password</param>
    /// <returns>App user</returns>
    Task<AppUser> RegisterAsync(Auth_RegisterRequest request);

    /// <summary>
    /// Login by username or email adress and password. After successfully login, token gets returned to authenticate.
    /// </summary>
    /// <param name="request">Login request: Username/Email-adress and password</param>
    /// <returns>JWT Token with expiration time</returns>
    Task<JwtTokenResult> LoginAsync(Auth_LoginRequest request);

    /// <summary>
    /// Refreshes an expired or soon-to-expire access token using a valid refresh token.
    /// </summary>
    /// <param name="refreshTokenValue">The refresh token provided by the client.</param>
    /// <returns>
    /// A <see cref="JwtTokenResult"/> containing a new access token,
    /// its expiration, and optionally a new refresh token with its expiration.
    /// </returns>
    /// <exception cref="ValidationException">
    /// Thrown if the refresh token is invalid, revoked, or expired.
    /// </exception>
    Task<JwtTokenResult> RefreshAsync(string refreshTokenValue);
}