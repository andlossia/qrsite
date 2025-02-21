using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;
        private readonly AppDbContext _context;


   public AuthController(AuthService authService, JwtService jwtService, AppDbContext context)
{
    _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
    _context = context ?? throw new ArgumentNullException(nameof(context));
}

[AllowAnonymous]
 [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody] UserLoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
            return BadRequest(new { Message = "Email and password are required." });

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.IsAdmin);
        
    return Ok(new
    {
        Success = true,
        Message = "User signed in successfully.",
        User = new
        {
            user.Id,
            user.Username,
            user.Email,
            user.IsAdmin,
            Role = user.Role.ToString(),
            Status = user.Status.ToString(),
            user.CreatedAt,
            user.UpdatedAt
        },
        Token = token
    });
}


    private bool VerifyPassword(string password, string storedHash)
    {
        using var sha256 = SHA256.Create();
        var computedHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return computedHash == storedHash;
    }

    [AllowAnonymous]

[HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] UserRegisterDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Username) || 
            string.IsNullOrWhiteSpace(registerDto.Email) || 
            string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return BadRequest(new { Message = "All fields are required." });
        }

        var userExists = await _context.Users.AnyAsync(u => u.Email == registerDto.Email);
        if (userExists)
        {
            return Conflict(new { Message = "User with the same email already exists." });
        }

        var newUser = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = HashPassword(registerDto.Password),
            IsAdmin = registerDto.IsAdmin,
            Role = registerDto.IsAdmin ? UserRole.Admin : UserRole.Participant
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User registered successfully", User = new { newUser.Id, newUser.Username, newUser.Email } });
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
            return BadRequest(new { Message = "Email is required." });

        var token = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
        if (token == null)
            return NotFound(new { Message = "User not found." });

        return Ok(new { ResetToken = token });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ResetToken) || string.IsNullOrWhiteSpace(dto.NewPassword))
            return BadRequest(new { Message = "Reset token and new password are required." });

        var success = await _authService.ResetPasswordAsync(dto.ResetToken, dto.NewPassword);
        if (!success)
            return BadRequest(new { Message = "Invalid or expired reset token." });

        return Ok(new { Message = "Password reset successfully" });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { Message = "Invalid token." });

        var user = await _authService.GetProfileAsync(userId);
        if (user == null)
            return NotFound(new { Message = "User profile not found." });

        return Ok(user);
    }

   [Authorize]
[HttpPost("logout")]
public async Task<IActionResult> Logout()
{
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    if (string.IsNullOrWhiteSpace(token))
        return BadRequest(new { Message = "Token is required." });

    var success = await _jwtService.RevokeTokenAsync(token);
    if (!success)
        return BadRequest(new { Message = "Invalid token or already logged out." });

    return Ok(new { Message = "Logged out successfully." });
}

[HttpPost("validate-token")]
public IActionResult ValidateToken([FromBody] string token)
{
    if (string.IsNullOrWhiteSpace(token))
        return BadRequest(new { Message = "Token is required." });

    var isValid = _jwtService.ValidateToken(token);
    if (!isValid)
        return Unauthorized(new { Message = "Invalid or expired token." });

    return Ok(new { Message = "Token is valid." });
}

}
