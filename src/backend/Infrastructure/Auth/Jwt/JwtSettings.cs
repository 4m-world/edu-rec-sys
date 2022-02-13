namespace CodeMatrix.Mepd.Infrastructure.Auth.Jwt;

public class JwtSettings
{
    public string Key { get; set; } = default;

    public int TokenExpirationInMinutes { get; set; }

    public int RefreshTokenExpirationInDays { get; set; }
}