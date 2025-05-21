using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class MovieCast
{
    [Key]
    public int MovieCastId { get; set; }

    [Required]
    public int MovieId { get; set; }

    [Required]
    public int PersonId { get; set; }

    [StringLength(100)]
    public string CharacterName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    [ForeignKey("MovieId")]
    public virtual Movie Movie { get; set; }

    [ForeignKey("PersonId")]
    public virtual MoviePerson Person { get; set; }

    public virtual ICollection<MovieCastRoleType> MovieCastRoleTypes { get; set; } = new List<MovieCastRoleType>();
}
