using RESTful.Entity;

namespace RESTful.Auth.Interface;

public interface IJwtTokenService
{
    (string token, DateTime expiresAtUtc) CreateToken(AppUser user);
}
