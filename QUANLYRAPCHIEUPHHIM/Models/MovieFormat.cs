using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class MovieFormat
{
    [Key]
    public int MovieFormatId { get; set; }

    [Required]
    [ForeignKey("Movie")]
    public int MovieId { get; set; }

    [Required]
    [ForeignKey("RoomFormat")]
    public int RoomFormatId { get; set; }

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal PriceModifier { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public virtual Movie Movie { get; set; }
    public virtual RoomFormat RoomFormat { get; set; }
}
