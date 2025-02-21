public class EventSettingsService
{
    private readonly AppDbContext _context;

    public EventSettingsService(AppDbContext context)
    {
        _context = context;
    }

 public EventSettings AddSetting(EventSettings newSetting, long userId)
    {
        _context.EventSettings.Add(newSetting);
        _context.SaveChanges();
        return newSetting;
    }

    public EventSettings UpdateSetting(long id, EventSettings updatedSetting, long userId)
    {
        var existingSetting = _context.EventSettings.FirstOrDefault(s => s.Id == id);
        if (existingSetting == null) throw new KeyNotFoundException("Setting not found.");

        existingSetting.SettingKey = updatedSetting.SettingKey;
        existingSetting.SettingValue = updatedSetting.SettingValue;
        existingSetting.UpdatedAt = DateTime.UtcNow;

        _context.SaveChanges();
        return existingSetting;
    }

    public void DeleteSetting(long id, long userId)
    {
        var setting = _context.EventSettings.FirstOrDefault(s => s.Id == id);
        if (setting == null) throw new KeyNotFoundException("Setting not found.");

        _context.EventSettings.Remove(setting);
        _context.SaveChanges();
    }

    public EventSettings? GetSettingById(long id, long userId)
    {
        return _context.EventSettings.FirstOrDefault(s => s.Id == id);
    }
}