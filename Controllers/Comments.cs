using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CommentController : BaseController<Comment, long>
{
    public CommentController(AppDbContext context)
        : base(context, entity => entity.User) 
    {
    }

    [HttpPost]
    public override async Task<IActionResult> Create([FromBody] Comment entity)
    {
        if (CurrentUserId == null)
            return Unauthorized(new { Success = false, Message = "Unauthorized." });

        try
        {
            var user = await _context.Users.FindAsync(long.Parse(CurrentUserId));

            if (user == null)
{
    return BadRequest(new { Success = false, Message = "Invalid Organizer. User not found." });
}

            return await base.Create(entity);
        }
        catch (Exception ex)
        {
            return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
        }
    }

}
