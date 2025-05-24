using Microsoft.AspNetCore.Identity;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = null!;
    public DateTime RegistrationDate { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 