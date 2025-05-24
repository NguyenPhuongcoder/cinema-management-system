using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class MovieFormat
{
    public int MovieFormatId { get; set; }

    public int MovieId { get; set; }

    public int FormatId { get; set; }

    public virtual RoomFormat Format { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
