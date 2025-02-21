using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

public class AuthService
{
    private readonly JwtService _jwtService;
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

   public async Task<string?> SignInAsync(string email, string password)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user == null || !VerifyPassword(password, user.PasswordHash))
        return null;

    return _jwtService.GenerateToken(user.Id, user.Email, user.IsAdmin);
}


    public async Task<User?> SignUpAsync(User user)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            return null;

        user.PasswordHash = HashPassword(user.PasswordHash);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetProfileAsync(long id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<string?> ForgotPasswordAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return null;

        user.ResetToken = GenerateResetToken();
        user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync();
        return user.ResetToken;
    }

    public async Task<bool> ResetPasswordAsync(string resetToken, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetToken && u.ResetTokenExpiry > DateTime.UtcNow);
        if (user == null)
            return false;

        user.PasswordHash = HashPassword(newPassword);
        user.ResetToken = null;
        user.ResetTokenExpiry = null;

        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateResetToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == storedHash;
    }
}
