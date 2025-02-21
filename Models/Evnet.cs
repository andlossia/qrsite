using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public enum EventStatus
{
    Active,
    Inactive
}

public enum ContributorStatus
{
    Pending,
    Accepted,
    Declined
}

public enum ContributorsRole
{
    Admin,
    Contributor
}

public enum AttendanceStatus
{
    Pending,
    Accepted,
    Declined
}
public class Event
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    public string QREvent { get; set; } = string.Empty;
    public string QRAlbum { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public User? Organizer { get; set; }

    private long? _organizerId;

    [ForeignKey(nameof(Organizer))]
    public long? OrganizerId
    {
        get => Organizer?.Id ?? _organizerId; 
        set => _organizerId = value;          
    }

    public List<Content> Contents { get; set; } = new List<Content>();

    [NotMapped]
    public virtual List<EventContributor> Contributors { get; set; } = new List<EventContributor>();
    [NotMapped]
    public virtual List<EventAttendee> Attendees { get; set; } = new List<EventAttendee>();
    public EventSettings? Settings { get; set; }

    [NotMapped]
    public bool IsOngoing => StartDate <= DateTime.UtcNow && EndDate >= DateTime.UtcNow;

    [NotMapped]
    public bool IsUpcoming => StartDate > DateTime.UtcNow;

    [NotMapped]
    public bool IsPast => EndDate < DateTime.UtcNow;
}

public class EventContributor
{
    public required User User { get; set; }
    
    public ContributorStatus ContributorStatus { get; set; } = ContributorStatus.Pending;

    public ContributorsRole Role { get; set; } = ContributorsRole.Contributor;

}

public class EventAttendee
{

    public required User User { get; set; }

    public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Pending;

}
