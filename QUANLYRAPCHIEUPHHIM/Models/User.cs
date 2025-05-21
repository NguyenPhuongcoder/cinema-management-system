using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    [Required]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(20)]
    public string Phone { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [NotMapped]
    public string FullName
    {
        get => $"{FirstName} {LastName}";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                FirstName = null;
                LastName = null;
                return;
            }

            var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            FirstName = parts.Length > 0 ? parts[0][..Math.Min(parts[0].Length, 50)] : null;
            LastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1))[..Math.Min(string.Join(" ", parts.Skip(1)).Length, 50)] : null;
        }
    }
    [Required]
    public int RoleId { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public DateTime UpdatedAt { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool IsActive { get; set; }

    // Navigation properties
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
