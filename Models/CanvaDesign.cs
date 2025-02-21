using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum CanvaDesignStatus
{
    Active,
    Inactive,
    Deleted,
    PendingApproval,
    Approved,
    Rejected,
    Draft
}

public class CanvaDesign
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long CreatorId { get; set; }

    [ForeignKey("CreatorId")]
    public virtual User Creator { get; set; } = null!;

    [Required, MaxLength(255)]
    public string DesignName { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string DesignUrl { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? EmbedUrl { get; set; }

    [MaxLength(255)]
    public string? ThumbnailUrl { get; set; }

    public CanvaDesignStatus Status { get; set; } = CanvaDesignStatus.Draft;

    public string? Metadata { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
