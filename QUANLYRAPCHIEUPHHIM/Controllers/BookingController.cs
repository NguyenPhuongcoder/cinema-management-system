using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class BookingController : Controller
    {
        private readonly CinemaDbcontext _context;
        public BookingController(CinemaDbcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(int? provinceId, int? movieId, DateTime? selectedDate, int? selectedCinemaId)
        {
            var provinces = _context.Provinces.ToList();
            var selectedProvinceId = provinceId ?? provinces.FirstOrDefault()?.ProvinceId;

            // Lấy cityId thuộc province đã chọn
            var cityIds = _context.Cities
                .Where(city => city.ProvinceId == selectedProvinceId)
                .Select(city => city.CityId)
                .ToList();

            // Lấy addressId thuộc cityIds
            var addressIds = _context.Addresses
                .Where(addr => cityIds.Contains(addr.CityId))
                .Select(addr => addr.AddressId)
                .ToList();

            // Lấy cinemaId thuộc addressIds
            var cinemaIds = _context.Cinemas
                .Where(c => addressIds.Contains(c.AddressId))
                .Select(c => c.CinemaId)
                .ToList();

            // Lấy danh sách roomId thuộc các cinema này
            var roomIds = _context.Rooms
                .Where(r => cinemaIds.Contains(r.CinemaId))
                .Select(r => r.RoomId)
                .ToList();

            // Lấy danh sách phim có suất chiếu trong các room này
            var movies = _context.Movies
                .Where(m => m.Showtimes.Any(s => roomIds.Contains(s.RoomId)))
                .ToList();
            var selectedMovieId = movieId ?? movies.FirstOrDefault()?.MovieId;

            // Lấy danh sách suất chiếu theo phim và tỉnh
            var allShowtimes = new List<Showtime>();
            if (selectedMovieId != null)
            {
                allShowtimes = _context.Showtimes
                    .Where(s => s.MovieId == selectedMovieId && roomIds.Contains(s.RoomId))
                    .ToList();
            }

            // Lấy danh sách ngày có suất chiếu (chỉ lấy phần ngày)
            var availableDates = allShowtimes
                .Select(s => s.StartTime.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
            var selDate = selectedDate?.Date ?? availableDates.FirstOrDefault();

            // Lấy danh sách rạp có suất chiếu ngày đã chọn (so sánh theo ngày)
            var cinemaIdWithShow = allShowtimes
                .Where(s => s.StartTime.Date == selDate)
                .Select(s => _context.Rooms.FirstOrDefault(r => r.RoomId == s.RoomId)?.CinemaId ?? 0)
                .Distinct()
                .Where(id => id != 0)
                .ToList();
            var cinemas = _context.Cinemas.Where(c => cinemaIdWithShow.Contains(c.CinemaId)).ToList();
            var selCinemaId = selectedCinemaId ?? cinemas.FirstOrDefault()?.CinemaId;

            // Lọc suất chiếu theo ngày và rạp đã chọn (so sánh theo ngày)
            var showtimes = allShowtimes
                .Where(s => s.StartTime.Date == selDate &&
                            _context.Rooms.Any(r => r.RoomId == s.RoomId && r.CinemaId == selCinemaId))
                .OrderBy(s => s.StartTime)
                .ToList();

            var vm = new BookingPageViewModel
            {
                Provinces = provinces,
                SelectedProvinceId = selectedProvinceId,
                Movies = movies,
                SelectedMovieId = selectedMovieId,
                ShowtimeGroups = new List<ShowtimeGroupViewModel>(), // Không dùng nữa
                AvailableDates = availableDates,
                SelectedDate = selDate,
                Cinemas = cinemas,
                SelectedCinemaId = selCinemaId,
                Showtimes = showtimes
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Index(BookingPageViewModel model)
        {
            // Chuyển hướng về GET với các lựa chọn đã chọn để load lại dữ liệu
            return RedirectToAction("Index", new { provinceId = model.SelectedProvinceId, movieId = model.SelectedMovieId, selectedDate = model.SelectedDate?.ToString("yyyy-MM-dd"), selectedCinemaId = model.SelectedCinemaId });
        }
    }
} 