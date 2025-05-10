using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class MovieCastRoleType
{
    public int MovieCastRoleTypeId { get; set; }

    public int MovieCastId { get; set; }

    public int RoleTypeId { get; set; }

    public virtual MovieCast MovieCast { get; set; } = null!;

    public virtual RoleType RoleType { get; set; } = null!;
}
