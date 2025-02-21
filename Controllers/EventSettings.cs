
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EventSettingsController : ControllerBase
{
    private readonly EventSettingsService _settingsService;

    public EventSettingsController(EventSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [Authorize]
    [HttpPost]
    public IActionResult AddSetting([FromBody] EventSettings newSetting)
    {
        var userId = GetUserIdFromToken();
        if (userId <= 0)
            return Unauthorized(new { Message = "Invalid user token." });

        var addedSetting = _settingsService.AddSetting(newSetting, userId);
        return Ok(new { Message = "Setting added successfully", Data = addedSetting });
    }

    [Authorize]
    [HttpPut("{id}")]
    public IActionResult UpdateSetting(long id, [FromBody] EventSettings updatedSetting)
    {
        var userId = GetUserIdFromToken();
        if (userId <= 0)
            return Unauthorized(new { Message = "Invalid user token." });

        var settingResult = _settingsService.UpdateSetting(id, updatedSetting, userId);
        return Ok(new { Message = "Setting updated successfully", Data = settingResult });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public IActionResult DeleteSetting(long id)
    {
        var userId = GetUserIdFromToken();
        if (userId <= 0)
            return Unauthorized(new { Message = "Invalid user token." });

        _settingsService.DeleteSetting(id, userId);
        return Ok(new { Message = "Setting deleted successfully" });
    }

    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetSettingById(long id)
    {
        var userId = GetUserIdFromToken();
        if (userId <= 0)
            return Unauthorized(new { Message = "Invalid user token." });

        var settingResult = _settingsService.GetSettingById(id, userId);
        if (settingResult == null)
            return NotFound(new { Message = "Setting not found or access denied." });

        return Ok(new { Message = "Setting retrieved successfully", Data = settingResult });
    }

    private long GetUserIdFromToken()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity != null)
        {
            var userIdClaim = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
        }
        return 0;
    }
}
