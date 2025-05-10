using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class Cinema
{
    public int CinemaId { get; set; }

    public string CinemaName { get; set; } = null!;

    public int AddressId { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
