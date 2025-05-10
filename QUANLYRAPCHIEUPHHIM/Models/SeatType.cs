using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class SeatType
{
    public int SeatTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public decimal? AdditionalCharge { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}
