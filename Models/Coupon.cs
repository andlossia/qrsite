using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public enum CouponType
{
    Percent,
    Fixed
}

public enum CouponStatus
{
    Active,
    Inactive
}

public class Coupon
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Discount { get; set; } = 0;

    public CouponType Type { get; set; } = CouponType.Percent;

    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int UsageLimit { get; set; } = 0;
    public int TimesUsed { get; set; } = 0;

    public CouponStatus Status { get; set; } = CouponStatus.Active;

    public User? Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    public bool IsActive => Status == CouponStatus.Active && (ExpiresAt == null || ExpiresAt > DateTime.UtcNow);

    public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
}
