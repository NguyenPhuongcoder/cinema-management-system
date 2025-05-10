using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class TicketStatus
{
    public int TicketStatusId { get; set; }

    public string TicketStatusName { get; set; } = null!;

    public virtual ICollection<TicketTicketStatus> TicketTicketStatuses { get; set; } = new List<TicketTicketStatus>();
}
