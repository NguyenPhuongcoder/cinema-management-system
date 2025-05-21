using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class RoomFormat
{
    [Key]
    public int FormatId { get; set; }

    [Required]
    [StringLength(50)]
    public string FormatName { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal AdditionalCharge { get; set; }

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal BasePrice { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    public virtual ICollection<MovieFormat> MovieFormats { get; set; } = new List<MovieFormat>();
}
