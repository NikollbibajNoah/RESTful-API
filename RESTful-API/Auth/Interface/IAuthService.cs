using Microsoft.AspNetCore.Identity.Data;
using RESTful.Entity;
using RESTful.Entity.Auth;
using LoginRequest = RESTful.Entity.Auth.LoginRequest;
using RegisterRequest = RESTful.Entity.Auth.RegisterRequest;

namespace RESTful.Auth.Interface;

public interface IAuthService
{
    /// <summary>
    /// Register a new user by given register request which consists of unique name,
    /// email-adress and password. After successfully validation, registered user saved in database.
    /// </summary>
    /// <param name="request">Register request: username, email and password</param>
    /// <returns>App user</returns>
    Task<AppUser> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Login by username or email adress and password. After successfully login, token gets returned to authenticate.
    /// </summary>
    /// <param name="request">Login request: Username/Email-adress and password</param>
    /// <returns>JWT Token with expiration time</returns>
    Task<JwtTokenResult> LoginAsync(LoginRequest request);

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