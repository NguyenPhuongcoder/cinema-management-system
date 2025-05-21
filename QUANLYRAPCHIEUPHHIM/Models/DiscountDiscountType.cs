using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class DiscountDiscountType
{
    [Key]
    public int DiscountDiscountTypeId { get; set; }

    [Required]
    [ForeignKey("Discount")]
    public int DiscountId { get; set; }

    [Required]
    [ForeignKey("DiscountType")]
    public int DiscountTypeId { get; set; }

    // Navigation properties
    public Discount Discount { get; set; }
    public DiscountType DiscountType { get; set; }
}
