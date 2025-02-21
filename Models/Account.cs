using System.ComponentModel.DataAnnotations;

public enum UserRole
{
    Participant,
    Admin
}

public enum UserStatus
{
    Active,
    Inactive
}

public enum UserPlan
{
    Free,
    Premium
}

public class User
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    public bool IsAdmin { get; set; } = false;

    public UserRole Role { get; set; } = UserRole.Participant;

    public UserStatus Status { get; set; } = UserStatus.Active;
    public UserPlan Plan { get; set; } = UserPlan.Free;

    public string? ResetToken { get; set; }
    public DateTime? ResetTokenExpiry { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Content> UploadedContents { get; set; } = new HashSet<Content>();
    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    public virtual ICollection<Event> OrganizedEvents { get; set; } = new HashSet<Event>();
    public virtual ICollection<CanvaDesign> CanvaDesigns { get; set; } = new HashSet<CanvaDesign>();
  
}
