using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<CanvaDesign> CanvaDesigns { get; set; } = null!;
    public DbSet<Coupon> Coupons { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Content> Contents { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventSettings> EventSettings { get; set; } = null!;
    public DbSet<Media> Media { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Status).HasConversion<string>();
            entity.Property(u => u.Plan).HasConversion<string>();
            entity.Property(u => u.Role).HasConversion<string>();
        });

        modelBuilder.Entity<CanvaDesign>(entity =>
        {
            entity.ToTable("CanvaDesigns");
            entity.HasOne(d => d.Creator)
                  .WithMany(u => u.CanvaDesigns)
                  .HasForeignKey("CreatorId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Payments)
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Coupon)
                  .WithMany(c => c.Payments)
                  .HasForeignKey("CouponId")
                  .OnDelete(DeleteBehavior.SetNull);
            entity.Property(p => p.PaymentStatus).HasConversion<string>();
            entity.Property(p => p.PaymentMethod).HasConversion<string>();
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            entity.HasOne(rc => rc.User)
                .WithMany(u => u.Comments)
                .HasForeignKey("UserId")
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(rc => rc.Content)
                .WithMany(c => c.Comments)
                .HasForeignKey("ContentId")
                .OnDelete(DeleteBehavior.Cascade);
        });

       modelBuilder.Entity<Content>(entity =>
{
    entity.ToTable("Contents");

    entity.HasOne(c => c.Event)
          .WithMany(e => e.Contents)
          .HasForeignKey(c => c.EventId) 
          .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(c => c.Uploader)
          .WithMany(u => u.UploadedContents)
          .HasForeignKey(c => c.UploaderId) 
          .OnDelete(DeleteBehavior.Cascade);

    entity.Property(c => c.Type)
          .HasConversion<string>()
          .IsRequired(); 

    entity.HasIndex(c => new { c.EventId, c.UploaderId, c.Type })
          .IsUnique(); 
});


        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Events");
            entity.HasOne(e => e.Organizer)
                  .WithMany(u => u.OrganizedEvents)
                  .HasForeignKey("OrganizerId")
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Status).HasConversion<string>();
        });

        modelBuilder.Entity<EventSettings>(entity =>
        {
            entity.ToTable("EventSettings");
            entity.HasOne(es => es.Event)
                  .WithOne(e => e.Settings)
                  .HasForeignKey<EventSettings>("EventId")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.ToTable("Coupons");
            entity.HasOne(c => c.Owner)
                  .WithMany()
                  .HasForeignKey("OwnerId")
                  .OnDelete(DeleteBehavior.SetNull);
            entity.Property(c => c.Type).HasConversion<string>();
            entity.Property(c => c.Status).HasConversion<string>();
        });

        modelBuilder.Entity<Media>(entity =>
        {
            entity.ToTable("MediaFiles");
        });
    }
}
