using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Users
 [HttpGet]
public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
{
    var users = await _context.Users
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var totalUsers = await _context.Users.CountAsync();

    return Ok(new
    {
        Message = "All Users",
        Data = users,
        Pagination = new
        {
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalUsers,
            TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize)
        }
    });
}


    // POST: api/Users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user)
    {
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            return BadRequest(new { Message = "User with this email already exists." });
        }

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "User created", User = user });
    }

    // GET: api/Users/{Id}
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetUserById(long Id)
    {
        var user = await _context.Users.FindAsync(Id);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {Id} not found" });
        }
        return Ok(new { Message = "User found", Data = user });
    }

    // PUT: api/Users/{Id}
    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateUser(long Id, [FromBody] User user)
    {
        var existingUser = await _context.Users.FindAsync(Id);
        if (existingUser == null)
        {
            return NotFound(new { Message = $"User with ID {Id} not found" });
        }

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
        existingUser.IsAdmin = user.IsAdmin;
        existingUser.Role = user.Role;
        existingUser.Status = user.Status;
        existingUser.Plan = user.Plan;

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User updated", UpdatedUser = existingUser });
    }

    // DELETE: api/Users/{UserId}
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteUser(long Id)
    {
        var user = await _context.Users.FindAsync(Id);
        if (user == null)
        {
            return NotFound(new { Message = $"User with ID {Id} not found" });
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User deleted" });
    }

    // POST: api/Users/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkCreateUsers([FromBody] List<User> users)
    {
        var emails = users.Select(u => u.Email).ToList();
        var existingUsers = await _context.Users
            .Where(u => emails.Contains(u.Email))
            .ToListAsync();

        if (existingUsers.Any())
        {
            return BadRequest(new { Message = "Some users already exist", ExistingUsers = existingUsers });
        }

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Bulk Users created", Users = users });
    }

    // DELETE: api/Users/bulk
    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDeleteUsers([FromBody] List<long> ids)
    {
        var usersToDelete = await _context.Users.Where(u => ids.Contains(u.Id)).ToListAsync();
        if (!usersToDelete.Any())
        {
            return NotFound(new { Message = "No users found for the given IDs" });
        }

        _context.Users.RemoveRange(usersToDelete);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Bulk Users deleted", DeletedIds = ids });
    }

    // GET: api/Users/admins
    [HttpGet("admins")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var admins = await _context.Users.Where(u => u.IsAdmin).ToListAsync();
        return Ok(new { Message = "All Admins", Data = admins });
    }
}
