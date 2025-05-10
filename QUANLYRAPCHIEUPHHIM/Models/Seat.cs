using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class Seat
{
    public int SeatId { get; set; }

    public int RoomId { get; set; }

    public int SeatTypeId { get; set; }

    public string RowLetter { get; set; } = null!;

    public int SeatNumber { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual SeatType SeatType { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
