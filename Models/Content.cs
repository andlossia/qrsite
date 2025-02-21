using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum ContentType
{
    Note,
    Image,
    Video
}
public class Content
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long EventId { get; set; }

    [ForeignKey("EventId")]
    public Event Event { get; set; } = null!;

    [Required]
    public long UploaderId { get; set; } 

    [ForeignKey("UploaderId")]
    public User Uploader { get; set; } = null!;

    public ContentType Type { get; set; } = ContentType.Note;

    public string? TextContent { get; set; }
    public Media? Media { get; set; }

    public List<Comment> Comments { get; set; } = new List<Comment>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}



public class Reactions
{
    public int Like { get; set; } = 0;
    public int Love { get; set; } = 0;
    public int Laugh { get; set; } = 0;
}
