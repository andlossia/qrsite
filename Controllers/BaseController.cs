using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class BaseController<TEntity, TKey> : ControllerBase where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly Func<TEntity, object?> _ownerResolver;

    // Configuration for anonymous access
    private readonly bool _allowAnonymous;

    public BaseController(AppDbContext context, Func<TEntity, object?> ownerResolver, bool allowAnonymous = false)
    {
        _context = context;
        _ownerResolver = ownerResolver;
        _allowAnonymous = allowAnonymous;
    }

    protected string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    private bool IsAdmin => User.FindFirstValue(ClaimTypes.Role) == "Admin";

    private IActionResult UnauthorizedResponse() => Unauthorized(new { Success = false, Message = "Unauthorized." });

    public class BulkRequest<T>
    {
        public IEnumerable<T> Entities { get; set; } = new List<T>();
    }

    // GET: api/[controller]
  [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? filter = null,
        [FromQuery] string? sort = "Id",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        const int MaxPageSize = 100;
        if (page < 1 || pageSize < 1 || pageSize > MaxPageSize)
            return BadRequest(new { Success = false, Message = $"Page and PageSize must be greater than zero and PageSize must not exceed {MaxPageSize}." });

        try
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (!string.IsNullOrEmpty(filter) && !ValidateHelper<TEntity>.ValidateQuery(_context, filter))
                return BadRequest(new { Success = false, Message = "Invalid filter expression." });

            if (!string.IsNullOrEmpty(sort) && !ValidateHelper<TEntity>.ValidateSort(_context, sort))
                return BadRequest(new { Success = false, Message = "Invalid sort expression." });

            query = !string.IsNullOrEmpty(filter) ? query.Where(filter) : query;
            query = !string.IsNullOrEmpty(sort) ? query.OrderBy(sort) : query;

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

    // GET: api/[controller]/mine   
[HttpGet("mine")]
public async Task<IActionResult> GetMine(
    [FromQuery] string? filter = null,
    [FromQuery] string? sort = "Id",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
{
    const int MaxPageSize = 100;
    if (page < 1 || pageSize < 1 || pageSize > MaxPageSize)
        return BadRequest(new { Success = false, Message = $"Page and PageSize must be greater than zero and PageSize must not exceed {MaxPageSize}." });

    try
    {
        var query = _context.Set<TEntity>().AsQueryable();

        query = query.Where(e => _ownerResolver(e)!.ToString() == CurrentUserId);

             if (!string.IsNullOrEmpty(filter) && !ValidateHelper<TEntity>.ValidateQuery(_context, filter))
                return BadRequest(new { Success = false, Message = "Invalid filter expression." });

            if (!string.IsNullOrEmpty(sort) && !ValidateHelper<TEntity>.ValidateSort(_context, sort))
                return BadRequest(new { Success = false, Message = "Invalid sort expression." });

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

    // GET: api/[controller]/{id}
   // BaseController.cs
[HttpGet("{id}")]
public virtual async Task<IActionResult> GetById(TKey id)
{
    try
    {
        var entity = await _context.Set<TEntity>().FindAsync(id);
        if (entity == null)
            return NotFound(new { Success = false, Message = "Entity not found." });

        return Ok(new { Success = true, Data = entity });
    }
    catch (Exception ex)
    {
        return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
    }
}



    // POST: api/[controller]
  [HttpPost]
public virtual async Task<IActionResult> Create([FromBody] TEntity entity)
{
    if (entity == null)
        return BadRequest(new { Success = false, Message = "Invalid input data." });

    try
    {
        ValidateHelper<TEntity>.ValidateRequiredFields(entity);
        RelationshipHelper<TEntity>.ResolveRelationships(_context, entity);


        if (_allowAnonymous)
        {
        OwnerHelper<TEntity>.SetAnonymousOwner(entity, HttpContext.Items["AnonymousToken"]?.ToString());

        }
        else
        {
    await OwnerHelper<TEntity>.EnsureAuthorizedOwner(entity, CurrentUserId, _context, _ownerResolver);

        }

        _context.Set<TEntity>().Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = GetEntityKey(entity) }, new { Success = true, Data = entity });
    }
    catch (Exception ex)
    {
 return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

    }
}

    // POST: api/[controller]/bulk
[HttpPost("bulk")]
public virtual async Task<IActionResult> BulkCreate([FromBody] BulkRequest<TEntity> request)
{
    if (request.Entities == null || !request.Entities.Any())
        return BadRequest(new { Success = false, Message = "No entities provided." });

    var validEntities = new List<TEntity>();
    var errors = new List<string>();

    foreach (var entity in request.Entities)
    {
        try
        {
            ValidateHelper<TEntity>.ValidateRequiredFields(entity);
            RelationshipHelper<TEntity>.ResolveRelationships(_context, entity);

            if (_allowAnonymous)
            {
            OwnerHelper<TEntity>.SetAnonymousOwner(entity, HttpContext.Items["AnonymousToken"]?.ToString());
            }
            else
            {
        await OwnerHelper<TEntity>.EnsureAuthorizedOwner(entity, CurrentUserId, _context, _ownerResolver);


            }

            validEntities.Add(entity);
        }
        catch (Exception ex)
{
    return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

}

    }

    if (!validEntities.Any())
        return BadRequest(new { Success = false, Message = "All entities failed validation.", Errors = errors });

    try
    {
        await _context.Set<TEntity>().AddRangeAsync(validEntities);
        await _context.SaveChangesAsync();

        return Ok(new { Success = true, Data = validEntities, Errors = errors });
    }
    catch (Exception ex)
    {
    return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

    }
}


    // PUT: api/[controller]/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(TKey id, [FromBody] TEntity entity)
    {
        if (entity == null)
            return BadRequest(new { Success = false, Message = "Invalid input data." });

        try
        {
            var existingEntity = await _context.Set<TEntity>().FindAsync(id);
            if (existingEntity == null)
                return NotFound(new { Success = false, Message = "Entity not found." });

            var owner = _ownerResolver(existingEntity);
            if (!_allowAnonymous && owner?.ToString() != CurrentUserId && !IsAdmin)
                return UnauthorizedResponse();

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Data = entity });
        }
        catch (Exception ex)
        {
            return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

        }
    }

    // DELETE: api/[controller]/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(TKey id)
    {
        try
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
                return NotFound(new { Success = false, Message = "Entity not found." });

            var owner = _ownerResolver(entity);
            string anonymousToken = HttpContext.Items["AnonymousToken"]?.ToString() ?? string.Empty;

            if (!_allowAnonymous || (owner?.ToString() != anonymousToken && owner?.ToString() != CurrentUserId && !IsAdmin))
                return UnauthorizedResponse();

            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Entity deleted." });
        }
        catch (Exception ex)
        {
    return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");

        }
    }

    // DELETE: api/[controller]/bulk
    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDelete([FromBody] IEnumerable<TKey> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest(new { Success = false, Message = "No IDs provided." });

        try
        {
            var entities = await _context.Set<TEntity>().Where(e => ids.Contains(GetEntityKey(e))).ToListAsync();

            string anonymousToken = HttpContext.Items["AnonymousToken"]?.ToString() ?? string.Empty;

            if (!IsAdmin && entities.Any(e => _ownerResolver(e)?.ToString() != anonymousToken && _ownerResolver(e)?.ToString() != CurrentUserId))
                return UnauthorizedResponse();

            _context.Set<TEntity>().RemoveRange(entities);
            await _context.SaveChangesAsync();

            return Ok(new { Success = true, Message = "Entities deleted." });
        }
        catch (Exception ex)
        {
    return ApiErrorHandler.HandleApiError(ex, Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
        }
    }

    private TKey GetEntityKey(TEntity entity)
    {
        var keyProperty = entity.GetType().GetProperty("Id") ?? throw new InvalidOperationException("Entity must have an 'Id' property.");
        return (TKey)keyProperty.GetValue(entity, null)!;
    }

}
