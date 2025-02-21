using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

[ApiController]
[Route("api/[controller]")]
public class MediaController : BaseController<Media, long>
{
    public MediaController(AppDbContext  context) : base(context, entity => entity.Event?.Organizer?.Id,  allowAnonymous: true)
    {
    }

    [HttpGet("by-event/{eventId}")]
    public async Task<IActionResult> GetMediaByEvent(
        long eventId,
        [FromQuery] string? filter = null,
        [FromQuery] string? sort = "UploadedAt desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Set<Media>().Where(m => m.Event.Id == eventId);

            // Apply dynamic filtering and sorting
            query = ApplyDynamicQuery(query, filter, sort);

            // Pagination
            if (page < 1 || pageSize < 1)
                return BadRequest(new { Success = false, Message = "Page and PageSize must be greater than zero." });

            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = items,
                Pagination = new
                {
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                }
            });
        }
        catch (Exception ex)
        {
           return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

        }
    }

    [HttpGet("pending-approval")]
    public async Task<IActionResult> GetPendingApprovalMedia(
        [FromQuery] string? filter = null,
        [FromQuery] string? sort = "UploadedAt desc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Set<Media>().Where(m => m.ApprovalStatus == ApprovalStatus.Pending);

            // Apply dynamic filtering and sorting
            query = ApplyDynamicQuery(query, filter, sort);

            // Pagination
            if (page < 1 || pageSize < 1)
                return BadRequest(new { Success = false, Message = "Page and PageSize must be greater than zero." });

            var totalItems = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                Success = true,
                Data = items,
                Pagination = new
                {
                    TotalItems = totalItems,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                }
            });
        }
        catch (Exception ex)
        {
                return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

        }
    }

    [HttpPut("{id}/approval")]
    public async Task<IActionResult> UpdateApprovalStatus(long id, [FromBody] ApprovalStatus approvalStatus)
    {
        try
        {
            var media = await _context.Set<Media>().FindAsync(id);
            if (media == null)
                return NotFound(new { Success = false, Message = "Media not found." });

            media.ApprovalStatus = approvalStatus;
            media.UpdatedAt = DateTime.UtcNow;

            _context.Entry(media).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Data = media });
        }
        catch (Exception ex)
        {
                return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

        }
    }

    private IQueryable<Media> ApplyDynamicQuery(IQueryable<Media> query, string? filter, string? sort)
    {
        if (!string.IsNullOrEmpty(filter))
        {
            try
            {
                query = query.Where(filter);
            }
            catch
            {
                throw new ArgumentException("Invalid filter expression.");
            }
        }

        if (!string.IsNullOrEmpty(sort))
        {
            try
            {
                query = query.OrderBy(sort);
            }
            catch
            {
                throw new ArgumentException("Invalid sort expression.");
            }
        }

        return query;
    }
}
