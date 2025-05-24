using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Discount
{
    public int DiscountId { get; set; }

    public string DiscountName { get; set; } = null!;

    public decimal DiscountValue { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? CouponCode { get; set; }

    public bool? IsActive { get; set; }

    public int? UsageLimit { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<DiscountDiscountType> DiscountDiscountTypes { get; set; } = new List<DiscountDiscountType>();
}
