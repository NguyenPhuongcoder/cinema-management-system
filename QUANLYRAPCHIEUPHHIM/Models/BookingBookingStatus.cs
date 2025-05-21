using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class BookingBookingStatus
{
    [Key]
    public int BookingBookingStatusId { get; set; }

    [Required]
    public int BookingId { get; set; }

    [Required]
    public int BookingStatusId { get; set; }

    [Required]
    public DateTime StatusDate { get; set; }

    // Navigation properties
    [ForeignKey("BookingId")]
    public virtual Booking Booking { get; set; }

    [ForeignKey("BookingStatusId")]
    public virtual BookingStatus BookingStatus { get; set; }
}
