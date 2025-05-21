using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Payment
{
    [Key]
    public int PaymentId { get; set; }

    [Required]
    public int BookingId { get; set; }

    [Required]
    public int PaymentMethodId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    [StringLength(100)]
    public string PaymentStatus { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("BookingId")]
    public virtual Booking Booking { get; set; }

    [ForeignKey("PaymentMethodId")]
    public virtual PaymentMethod PaymentMethod { get; set; }
}
