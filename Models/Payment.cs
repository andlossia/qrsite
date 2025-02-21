using System.ComponentModel.DataAnnotations;

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed
}

public enum PaymentMethod
{
    CreditCard,
    PayPal,
    BankTransfer
}

public class Payment
{
    [Key]
    public long Id { get; set; }

    [Required]
    public User User { get; set; } = null!;

    public Coupon? Coupon { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }

    [Required, MaxLength(100)]
    public string PaymentReference { get; set; } = string.Empty;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

    public string? Metadata { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
