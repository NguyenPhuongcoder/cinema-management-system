using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class DiscountDiscountType
{
    public int DiscountDiscountTypeId { get; set; }

    public int DiscountId { get; set; }

    public int DiscountTypeId { get; set; }

    public virtual Discount Discount { get; set; } = null!;

    public virtual DiscountType DiscountType { get; set; } = null!;
}
