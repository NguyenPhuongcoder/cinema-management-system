using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Address
{
    [Key]
    public int AddressId { get; set; }

    [Required]
    [StringLength(200)]
    public string Street { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; }

    [Required]
    [StringLength(100)]
    public string State { get; set; }

    [Required]
    [StringLength(20)]
    public string ZipCode { get; set; }

    [Required]
    [StringLength(100)]
    public string Country { get; set; }

    [StringLength(500)]
    public string? AddressDetail { get; set; }

    public int? CityId { get; set; }

    // Navigation properties
    [ForeignKey("CityId")]
    public virtual City? CityEntity { get; set; }

    public virtual ICollection<Cinema> Cinemas { get; set; } = new List<Cinema>();
}
