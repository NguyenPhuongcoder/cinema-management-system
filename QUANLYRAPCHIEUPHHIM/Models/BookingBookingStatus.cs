using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class BookingBookingStatus
{
    public int BookingBookingStatusId { get; set; }

    public int BookingId { get; set; }

    public int BookingStatusId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual BookingStatus BookingStatus { get; set; } = null!;
}
