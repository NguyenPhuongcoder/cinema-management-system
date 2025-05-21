using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Ticket
{
    [Key]
    public int TicketId { get; set; }

    [Required]
    public int BookingId { get; set; }

    [Required]
    public int ShowtimeId { get; set; }

    [Required]
    public int SeatId { get; set; }

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(20)]
    public string TicketCode { get; set; }

    public DateTime? ScanDatetime { get; set; }

    [StringLength(40)]
    public string TicketStatus { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("BookingId")]
    public virtual Booking Booking { get; set; }

    [ForeignKey("ShowtimeId")]
    public virtual Showtime Showtime { get; set; }

    [ForeignKey("SeatId")]
    public virtual Seat Seat { get; set; }
}
