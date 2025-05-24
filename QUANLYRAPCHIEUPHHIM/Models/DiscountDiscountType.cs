using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class DiscountDiscountType
{
    public int DiscountDiscountTypeId { get; set; }

    public int DiscountId { get; set; }

    public int DiscountTypeId { get; set; }

    public virtual Discount Discount { get; set; } = null!;

    public virtual DiscountType DiscountType { get; set; } = null!;
}
