using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QUANLYRAPCHIEUPHHIM.Models;

public class Movie
{
    [Key]
    public int MovieId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public int Duration { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public string? Description { get; set; }

    [StringLength(10)]
    public string? AgeLimit { get; set; }

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal BasePrice { get; set; }

    [StringLength(255)]
    public string? PosterUrl { get; set; }

    [StringLength(255)]
    public string? PanelUrl { get; set; }

    [StringLength(255)]
    public string? TrailerUrl { get; set; }

    public float? Rating { get; set; }

    [StringLength(50)]
    public string? Language { get; set; }

    [StringLength(50)]
    public string? Subtitles { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    public virtual ICollection<MovieFormat> MovieFormats { get; set; } = new List<MovieFormat>();
    public virtual ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
