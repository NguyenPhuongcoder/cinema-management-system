using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models;

public partial class Movie
{
    public int MovieId { get; set; }

    public string Title { get; set; } = null!;

    public int Duration { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? Description { get; set; }

    public string? AgeLimit { get; set; }

    public decimal BasePrice { get; set; }

    public string? PosterUrl { get; set; }

    public string? PanelUrl { get; set; }

    public string? TrailerUrl { get; set; }

    public double? Rating { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Language { get; set; }

    public string? Subtitles { get; set; }

    public virtual ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();

    public virtual ICollection<MovieFormat> MovieFormats { get; set; } = new List<MovieFormat>();

    public virtual ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
