using RESTful.Entity;
using RESTful.Entity.Auth;

namespace RESTful.Auth.Interface;

public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for the given user with claims and expiration.
    /// </summary>
    /// <param name="user">The user for whom the token is generated.</param>
    /// <returns>A JwtTokenResult containing the token string and expiration time.</returns>
    JwtTokenResult CreateToken(AppUser user);
}
