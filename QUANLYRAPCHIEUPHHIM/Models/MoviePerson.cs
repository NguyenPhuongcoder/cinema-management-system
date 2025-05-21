using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class MoviePerson
{
    [Key]
    public int PersonId { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; }

    public DateTime? BirthDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Nationality { get; set; }

    [Required]
    [StringLength(1000)]
    public string Biography { get; set; }

    public string? PhotoUrl { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
}
