using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class MovieFormat
{
    public int MovieFormatId { get; set; }

    public int MovieId { get; set; }

    public int FormatId { get; set; }

    public virtual RoomFormat Format { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
