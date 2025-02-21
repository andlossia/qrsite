using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class EventSettings
{
    [Key]
    public long Id { get; set; }

    [Required]
    [ForeignKey(nameof(Event))]
    public long EventId { get; set; }

    [Required, MaxLength(255)]
    public string SettingKey { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string SettingValue { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual Event Event { get; set; } = null!;
}
