using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class RoomFormat
{
    public int FormatId { get; set; }

    public string FormatName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? AdditionalCharge { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<MovieFormat> MovieFormats { get; set; } = new List<MovieFormat>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
