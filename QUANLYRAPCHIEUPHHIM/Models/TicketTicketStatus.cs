using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class TicketTicketStatus
{
    public int TicketTicketStatusId { get; set; }

    public int TicketId { get; set; }

    public int TicketStatusId { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual TicketStatus TicketStatus { get; set; } = null!;
}
