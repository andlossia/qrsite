using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContentController : BaseController<Content, long>
{
    public ContentController(AppDbContext context)
        : base(context, content => content.Uploader.Id, allowAnonymous: true)
    {
    }

    [HttpPost]
    public override async Task<IActionResult> Create([FromBody] Content content)
    {
        if (content == null)
            return BadRequest(new { Success = false, Message = "Invalid content data." });

        try
        {
            // Validate EventId and fetch the associated Event
            var eventEntity = await _context.Events.FindAsync(content.EventId);
            if (eventEntity == null)
                return BadRequest(new { Success = false, Message = "Event not found." });

            // Validate UploaderId and fetch the associated User
            var uploader = await _context.Users.FindAsync(content.UploaderId);
            if (uploader == null)
                return BadRequest(new { Success = false, Message = "Uploader not found." });

            // Assign the related entities to the content
            content.Event = eventEntity;
            content.Uploader = uploader;

            // Save the Content
            _context.Contents.Add(content);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Data = content });
        }
        catch (Exception ex)
        {
            return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
        }
    }
}
