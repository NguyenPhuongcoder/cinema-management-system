using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Discount
{
    [Key]
    public int DiscountId { get; set; }

    [Required]
    [StringLength(50)]
    public string CouponCode { get; set; }

    [Required]
    [StringLength(100)]
    public string DiscountName { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountValue { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public int UsageLimit { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<DiscountDiscountType> DiscountDiscountTypes { get; set; } = new List<DiscountDiscountType>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
