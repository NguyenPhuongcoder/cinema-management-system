using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class MovieGenre
{
    public int MovieGenreId { get; set; }

    public int MovieId { get; set; }

    public int GenreId { get; set; }

    public virtual Genre Genre { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
