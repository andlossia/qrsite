using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EventsController : BaseController<Event, long>
{
    public EventsController(AppDbContext context)
        : base(context, entity => entity.Organizer) 
    {
    }

    [HttpPost]
    public override async Task<IActionResult> Create([FromBody] Event entity)
    {
        if (CurrentUserId == null)
            return Unauthorized(new { Success = false, Message = "Unauthorized." });

        try
        {
            if (entity.Organizer == null)
            {
                entity.Organizer = await _context.Users.FindAsync(long.Parse(CurrentUserId));
                if (entity.Organizer == null)
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
