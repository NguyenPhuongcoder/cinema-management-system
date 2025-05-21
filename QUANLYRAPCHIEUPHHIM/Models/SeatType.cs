using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class SeatType
{
    [Key]
    public int SeatTypeId { get; set; }

    [Required]
    [StringLength(50)]
    public string TypeName { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal AdditionalCharge { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
