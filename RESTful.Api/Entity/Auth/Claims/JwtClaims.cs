using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RESTful.Api.Entity.Auth.Claims;

public static class JwtClaims
{
    public const string Role = ClaimTypes.Role;
    public const string UserId = JwtRegisteredClaimNames.Sub;
    public const string Username = JwtRegisteredClaimNames.UniqueName;
    public const string Email = JwtRegisteredClaimNames.Email;
}
