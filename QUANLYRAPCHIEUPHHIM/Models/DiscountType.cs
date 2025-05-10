using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class DiscountType
{
    public int DiscountTypeId { get; set; }

    public string DiscountTypeName { get; set; } = null!;

    public virtual ICollection<DiscountDiscountType> DiscountDiscountTypes { get; set; } = new List<DiscountDiscountType>();
}
