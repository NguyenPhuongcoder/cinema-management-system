using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class City
{
    [Key]
    public int CityId { get; set; }

    [Required]
    [StringLength(100)]
    public string CityName { get; set; }

    public int ProvinceId { get; set; }
    [ForeignKey("ProvinceId")]
    public Province Province { get; set; }

    // Navigation properties
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}
