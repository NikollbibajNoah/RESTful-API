using RESTful.Api.Entity;
using RESTful.Api.Entity.Auth;

namespace RESTful.Api.Auth.Interface;

public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the given user with claims and expiration.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <returns>A JwtTokenResult containing the token string and expiration time.</returns>
    JwtTokenResult CreateToken(AppUser user);
}
