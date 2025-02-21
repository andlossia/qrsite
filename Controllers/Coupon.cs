using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

[ApiController]
[Route("api/[controller]")]
public class CouponController : BaseController<Coupon, long>
{
    public CouponController(AppDbContext  context) : base(context, entity => entity.Owner)
    {
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCoupons(
        [FromQuery] string? filter = null,
        [FromQuery] string? sort = "Id",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var query = _context.Set<Coupon>().Where(c => c.IsActive);

            query = ApplyDynamicQuery(query, filter, sort);

            if (page < 1 || pageSize < 1)
                return BadRequest(new { Success = false, Message = "Page and PageSize must be greater than zero." });

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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

    private IQueryable<Coupon> ApplyDynamicQuery(IQueryable<Coupon> query, string? filter, string? sort)
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
