using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class RoleType
{
    public int RoleTypeId { get; set; }

    public string RoleTypeName { get; set; } = null!;

    public virtual ICollection<MovieCastRoleType> MovieCastRoleTypes { get; set; } = new List<MovieCastRoleType>();
}
