using System.ComponentModel.DataAnnotations;

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected
}

public class Media
{
    [Key]
    public long Id { get; set; }

    [Required]
    public Event Event { get; set; } = null!;

    [Required]
    public User Uploader { get; set; } = null!;

    [Required, MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string FileUrl { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string FileType { get; set; } = string.Empty;

    public string? Metadata { get; set; }

    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
