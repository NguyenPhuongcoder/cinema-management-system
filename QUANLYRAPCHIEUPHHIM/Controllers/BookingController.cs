using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        public IActionResult Index(int? provinceId, int? movieId, DateTime? selectedDate, int? selectedCinemaId, int? selectedShowtimeId)
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
                ShowtimeGroups = new List<ShowtimeGroupViewModel>(),
                AvailableDates = availableDates,
                SelectedDate = selDate,
                Cinemas = cinemas,
                SelectedCinemaId = selCinemaId,
                Showtimes = showtimes,
                SelectedShowtimeId = selectedShowtimeId
            };
            return View("Booking", vm);
        }

        [HttpPost]
        public IActionResult Index(BookingPageViewModel model)
        {
            return RedirectToAction("Index", new {
                provinceId = model.SelectedProvinceId,
                movieId = model.SelectedMovieId,
                selectedDate = model.SelectedDate?.ToString("yyyy-MM-dd"),
                selectedCinemaId = model.SelectedCinemaId,
                selectedShowtimeId = model.SelectedShowtimeId
            });
        }

        public IActionResult SelectSeat(int selectedShowtimeId)
        {
            var showtime = _context.Showtimes.FirstOrDefault(s => s.ShowtimeId == selectedShowtimeId);
            if (showtime == null) return NotFound();
            var movie = _context.Movies.FirstOrDefault(m => m.MovieId == showtime.MovieId);
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == showtime.RoomId);
            var cinema = room != null ? _context.Cinemas.FirstOrDefault(c => c.CinemaId == room.CinemaId) : null;
            // Lấy danh sách ghế và trạng thái đã bán
            var seats = _context.Seats.Where(s => s.RoomId == room.RoomId).ToList();
            var soldSeatIds = _context.Tickets.Where(t => t.ShowtimeId == selectedShowtimeId).Select(t => t.SeatId).ToList();
            var seatTypes = _context.SeatTypes.ToDictionary(st => st.SeatTypeId, st => st.TypeName);
            var seatStatus = seats.Select(s => new SeatStatusViewModel
            {
                SeatId = s.SeatId,
                Row = s.RowLetter,
                Number = s.SeatNumber,
                IsSold = soldSeatIds.Contains(s.SeatId),
                Type = seatTypes.ContainsKey(s.SeatTypeId) ? seatTypes[s.SeatTypeId] : ""
            }).ToList();
            var vm = new BookingSeatViewModel
            {
                Movie = movie,
                Cinema = cinema,
                Showtime = showtime,
                Room = room,
                Seats = seatStatus
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Payment(int selectedShowtimeId, List<int> selectedSeatIds, string selectedPaymentMethod, string promoCode)
        {
            int userId = 0;
            if (User.Identity.IsAuthenticated)
            {
                int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out userId);
            }

            // Lấy giá showtime
            var showtime = _context.Showtimes.FirstOrDefault(s => s.ShowtimeId == selectedShowtimeId);
            decimal ticketPrice = showtime?.PriceModifier ?? 0;

            // Tính tổng tiền vé
            decimal totalAmount = ticketPrice * selectedSeatIds.Count;

            // Tạo booking trước với totalAmount đã biết
            var booking = new Booking
            {
                UserId = userId,
                BookingDate = DateTime.Now,
                TotalAmount = totalAmount
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges(); // Để lấy BookingId

            foreach (var seatId in selectedSeatIds)
            {
                var ticket = new Ticket
                {
                    BookingId = booking.BookingId,
                    ShowtimeId = selectedShowtimeId,
                    SeatId = seatId,
                    Price = ticketPrice,
                    TicketStatus = "Đã đặt",
                    TicketCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    CreatedAt = DateTime.Now
                };
                _context.Tickets.Add(ticket);
            }

            _context.SaveChanges();

            TempData["BookingSuccess"] = "Đặt vé thành công!";
            return RedirectToAction("Success");
        }


        public IActionResult Success()
        {
            ViewBag.Message = TempData["BookingSuccess"] ?? "Đặt vé thành công!";
            return View();
        }
    }
} 