using System.ComponentModel.DataAnnotations;

public class Comment
{
    [Key]
    public long Id { get; set; }

    [Required]
    public User User { get; set; } = null!;

    [Required, MaxLength(1000)]
    public string CommentText { get; set; } = string.Empty;

    [Required]
    public virtual Content Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
