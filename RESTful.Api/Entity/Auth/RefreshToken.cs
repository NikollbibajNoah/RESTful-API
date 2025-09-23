namespace RESTful.Api.Entity.Auth;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public int UserId { get; set; }

    public AppUser User { get; set; } = null!;
}
