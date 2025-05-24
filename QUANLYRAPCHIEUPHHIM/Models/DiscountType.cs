using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class DiscountType
{
    public int DiscountTypeId { get; set; }

    public string DiscountTypeName { get; set; } = null!;

    public virtual ICollection<DiscountDiscountType> DiscountDiscountTypes { get; set; } = new List<DiscountDiscountType>();
}
