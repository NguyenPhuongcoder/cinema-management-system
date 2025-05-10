using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public int ProvinceId { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Province Province { get; set; } = null!;
}
