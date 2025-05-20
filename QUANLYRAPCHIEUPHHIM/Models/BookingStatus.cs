using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class BookingStatus
{

    public int BookingStatusId { get; set; }

    public string BookingStatusName { get; set; } = null!;

    public virtual ICollection<BookingBookingStatus> BookingBookingStatuses { get; set; } = new List<BookingBookingStatus>();
}
