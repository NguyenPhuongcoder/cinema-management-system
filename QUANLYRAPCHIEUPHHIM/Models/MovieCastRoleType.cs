using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class MovieCastRoleType
{
    public int MovieCastRoleTypeId { get; set; }

    public int MovieCastId { get; set; }

    public int RoleTypeId { get; set; }

    public virtual MovieCast MovieCast { get; set; } = null!;

    public virtual RoleType RoleType { get; set; } = null!;
}
