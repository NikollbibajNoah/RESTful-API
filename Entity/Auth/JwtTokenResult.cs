using System.IdentityModel.Tokens.Jwt;

namespace RESTful.Entity.Auth;

public class JwtTokenResult
{
    public string AccessToken { get; }
    public DateTime AccessTokenExpiresAtUtc { get; }
    
    public string? RefreshToken { get; }
    
    public DateTime? RefreshTokenExpiresAtUtc { get; }

    public JwtTokenResult(string accessToken, DateTime accessTokenExpiresAtUtc)
    {
        AccessToken = accessToken;
        AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc;
    }
    
    public JwtTokenResult(string accessToken, DateTime accessTokenExpiresAtUtc, string refreshToken, DateTime refreshTokenExpiresAtUtc)
    {
        AccessToken = accessToken;
        AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc;
        RefreshToken = refreshToken;
        RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc;
    }

    public static JwtTokenResult FromToken(JwtSecurityToken token, string secret)
    {
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtTokenResult(jwt, token.ValidTo);
    }
}
