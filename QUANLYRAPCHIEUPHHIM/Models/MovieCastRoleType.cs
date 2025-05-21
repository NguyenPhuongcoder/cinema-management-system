using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class MovieCastRoleType
{
    [Key]
    public int MovieCastRoleTypeId { get; set; }

    [Required]
    public int MovieCastId { get; set; }

    [Required]
    public int RoleTypeId { get; set; }

    // Navigation properties
    [ForeignKey("MovieCastId")]
    public virtual MovieCast MovieCast { get; set; }

    [ForeignKey("RoleTypeId")]
    public virtual RoleType RoleType { get; set; }
}
