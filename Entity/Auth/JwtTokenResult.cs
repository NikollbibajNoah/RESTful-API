using System.IdentityModel.Tokens.Jwt;

namespace RESTful.Entity.Auth;

public class JwtTokenResult
{
    public string Token { get; }
    public DateTime ExpiresAtUtc { get; }

    public JwtTokenResult(string token, DateTime expiresAtUtc)
    {
        Token = token;
        ExpiresAtUtc = expiresAtUtc;
    }

    public static JwtTokenResult FromToken(JwtSecurityToken token, string secret)
    {
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new JwtTokenResult(jwt, token.ValidTo);
    }
}
