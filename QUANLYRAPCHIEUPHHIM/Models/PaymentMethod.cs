using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class PaymentMethod
{
    [Key]
    public int PaymentMethodId { get; set; }

    [Required]
    [StringLength(50)]
    public string MethodName { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
