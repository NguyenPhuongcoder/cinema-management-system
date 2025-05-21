using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class RoleType
{
    [Key]
    public int RoleTypeId { get; set; }

    [Required]
    [StringLength(50)]
    public string RoleTypeName { get; set; }

    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<MovieCastRoleType> MovieCastRoleTypes { get; set; } = new List<MovieCastRoleType>();
}
