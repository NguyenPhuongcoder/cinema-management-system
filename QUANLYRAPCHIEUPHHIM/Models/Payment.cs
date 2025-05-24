using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Payment
{
    public int PaymentId { get; set; }

    public int BookingId { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }
   // public string PaymentStatus { get; set; } = null!;
    public string? TransactionId { get; set; }

    public string? PaymentStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
