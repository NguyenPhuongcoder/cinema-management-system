using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class BookingPageViewModel
    {
        public List<Province> Provinces { get; set; }
        public int? SelectedProvinceId { get; set; }
        public List<Movie> Movies { get; set; }
        public int? SelectedMovieId { get; set; }
        public List<ShowtimeGroupViewModel> ShowtimeGroups { get; set; }
        public List<DateTime> AvailableDates { get; set; }
        public DateTime? SelectedDate { get; set; }
        public List<Cinema> Cinemas { get; set; }
        public int? SelectedCinemaId { get; set; }
        public List<Showtime> Showtimes { get; set; }
        public int? SelectedShowtimeId { get; set; }
    }
    public class ShowtimeGroupViewModel
    {
        [Required(ErrorMessage = "Tên rạp không được để trống")]
        public string CinemaName { get; set; }
        public List<Showtime> Showtimes { get; set; }
    }
} 