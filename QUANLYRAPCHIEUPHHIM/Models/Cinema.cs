using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Cinema
{
    [Key]
    public int CinemaId { get; set; }

    [Required]
    [StringLength(100)]
    public string CinemaName { get; set; }

    [Required]
    public int AddressId { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("AddressId")]
    public virtual Address Address { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
