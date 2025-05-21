using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Booking
{
    [Key]
    public int BookingId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public DateTime BookingDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    public int? DiscountId { get; set; }

    public DateTime? PaymentDueDate { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    [ForeignKey("DiscountId")]
    public virtual Discount? Discount { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public virtual ICollection<BookingBookingStatus> BookingBookingStatuses { get; set; } = new List<BookingBookingStatus>();
}
