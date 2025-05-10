using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class RoleType
{
    public int RoleTypeId { get; set; }

    public string RoleTypeName { get; set; } = null!;

    public virtual ICollection<MovieCastRoleType> MovieCastRoleTypes { get; set; } = new List<MovieCastRoleType>();
}
