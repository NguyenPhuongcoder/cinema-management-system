using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class MoviePerson
{
    public int PersonId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Nationality { get; set; }

    public string? Biography { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
}
