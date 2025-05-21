using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class DiscountType
{
    [Key]
    public int DiscountTypeId { get; set; }

    [Required]
    [StringLength(50)]
    public string DiscountTypeName { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<DiscountDiscountType> DiscountDiscountTypes { get; set; } = new List<DiscountDiscountType>();
}
