﻿using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class Room
{
    public int RoomId { get; set; }

    public int CinemaId { get; set; }

    public int FormatId { get; set; }

    public string RoomName { get; set; } = null!;

    public int Capacity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Cinema Cinema { get; set; } = null!;

    public virtual RoomFormat Format { get; set; } = null!;

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
