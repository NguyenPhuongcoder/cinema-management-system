using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public DateTime BookingDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int? DiscountId { get; set; }

    public DateTime? PaymentDueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BookingBookingStatus> BookingBookingStatuses { get; set; } = new List<BookingBookingStatus>();

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual User User { get; set; } = null!;
}
