using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class BookingBookingStatus
{

    public int BookingBookingStatusId { get; set; }

    public int BookingId { get; set; }

    public int BookingStatusId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual BookingStatus BookingStatus { get; set; } = null!;
}
