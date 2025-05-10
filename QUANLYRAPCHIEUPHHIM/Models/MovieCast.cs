using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class MovieCast
{
    public int MovieCastId { get; set; }

    public int MovieId { get; set; }

    public int PersonId { get; set; }

    public string? CharacterName { get; set; }

    public virtual Movie Movie { get; set; } = null!;

    public virtual ICollection<MovieCastRoleType> MovieCastRoleTypes { get; set; } = new List<MovieCastRoleType>();

    public virtual MoviePerson Person { get; set; } = null!;
}
