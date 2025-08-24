using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RESTful.Auth.Interface;
using RESTful.Entity;
using RESTful.Entity.Auth;
using RESTful.Entity.Auth.Claims;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace RESTful.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public JwtTokenResult CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtClaims.UserId, user.Id.ToString()),
            new Claim(JwtClaims.Username, user.Username),
            new Claim(JwtClaims.Email, user.Email),
            new Claim(JwtClaims.Role, user.Role ?? "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);
        
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );
        
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtTokenResult(jwt, expires);
    }
}