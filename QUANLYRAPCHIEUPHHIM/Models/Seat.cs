using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Seat
{
    [Key]
    public int SeatId { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    public int SeatTypeId { get; set; }

    [Required]
    [StringLength(1)]
    public string RowLetter { get; set; }

    [Required]
    public int SeatNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("RoomId")]
    public virtual Room Room { get; set; }

    [ForeignKey("SeatTypeId")]
    public virtual SeatType SeatType { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
