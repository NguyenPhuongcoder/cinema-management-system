using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Address
{
    public int AddressId { get; set; }

    public string AddressDetail { get; set; } = null!;

    public int CityId { get; set; }

    public virtual ICollection<Cinema> Cinemas { get; set; } = new List<Cinema>();

    public virtual City City { get; set; } = null!;

}
