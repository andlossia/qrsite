using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _expiryDuration;

    // In-memory storage for revoked tokens
    private readonly HashSet<string> _revokedTokens = new();

    public JwtService(string key, string issuer, string audience, TimeSpan expiryDuration)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            throw new ArgumentException("Key must be at least 32 characters long.", nameof(key));

        _key = key;
        _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        _audience = audience ?? throw new ArgumentNullException(nameof(audience));
        _expiryDuration = expiryDuration;
    }

    public string GenerateToken(long userId, string email, bool isAdmin)
    {
        if (userId <= 0)
            throw new ArgumentException("UserId must be greater than zero.", nameof(userId));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User"),
            new Claim("IsAdmin", isAdmin.ToString().ToLower()) // Custom claim
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(_expiryDuration),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<bool> RevokeTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));

        if (_revokedTokens.Contains(token))
            return Task.FromResult(false);

        _revokedTokens.Add(token);
        return Task.FromResult(true);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        if (_revokedTokens.Contains(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero 
            };

            tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
