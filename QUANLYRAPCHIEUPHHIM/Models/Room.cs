using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Room
{
    [Key]
    public int RoomId { get; set; }

    [Required]
    public int CinemaId { get; set; }

    [Required]
    [StringLength(50)]
    public string RoomName { get; set; }

    [Required]
    public int FormatId { get; set; }

    public int Capacity { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("CinemaId")]
    public virtual Cinema Cinema { get; set; }

    [ForeignKey("FormatId")]
    public virtual RoomFormat Format { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
