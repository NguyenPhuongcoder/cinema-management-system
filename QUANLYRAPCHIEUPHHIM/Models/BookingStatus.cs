using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class BookingStatus
{
    [Key]
    public int BookingStatusId { get; set; }

    [Required]
    [StringLength(50)]
    public string BookingStatusName { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<BookingBookingStatus> BookingBookingStatuses { get; set; } = new List<BookingBookingStatus>();
}
