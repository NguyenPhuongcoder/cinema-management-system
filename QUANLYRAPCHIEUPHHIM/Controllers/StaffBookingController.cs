using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class StaffBookingController : Controller
    {
        private readonly ILogger<StaffBookingController> _logger;
        private readonly CinemaDbContext _context;

        public StaffBookingController(ILogger<StaffBookingController> logger, CinemaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SelectMovie()
        {
            // Lấy danh sách phim từ CSDL
            var movies = await _context.Movies
                .Where(m => m.ReleaseDate <= DateOnly.FromDateTime(DateTime.Now))
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
            
            return View(movies);
        }

        public async Task<IActionResult> SelectShowtime(int movieId)
        {
            // Kiểm tra movieId
            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            // Lấy danh sách suất chiếu của phim đã chọn
            var showtimes = await _context.Showtimes
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.MovieId == movieId && s.StartTime > DateTime.Now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            ViewBag.Movie = movie;
            return View(showtimes);
        }

        public async Task<IActionResult> SelectSeats(int showtimeId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
                return NotFound();

            var roomSeats = await _context.Seats
                .Include(s => s.SeatType)
                .Where(s => s.RoomId == showtime.RoomId)
                .ToListAsync();

            var bookedSeats = await _context.Tickets
                .Where(t => t.ShowtimeId == showtimeId)
                .Select(t => t.SeatId)
                .ToListAsync();

            ViewBag.Showtime = showtime;
            ViewBag.BookedSeats = bookedSeats;
            ViewBag.BasePrice = showtime.Movie.BasePrice;

            return View(roomSeats);
        }

        public async Task<IActionResult> Checkout(int[] seatIds, int showtimeId)
        {
            if (seatIds == null || seatIds.Length == 0)
            {
                return RedirectToAction("SelectSeats", new { showtimeId });
            }

            // Lấy thông tin suất chiếu
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                return NotFound();
            }

            // Lấy thông tin ghế đã chọn
            var selectedSeats = await _context.Seats
                .Include(s => s.SeatType)
                .Where(s => seatIds.Contains(s.SeatId))
                .ToListAsync();

            // Lấy danh sách loại khuyến mãi
            var today = DateOnly.FromDateTime(DateTime.Now);
            var discounts = await _context.Discounts
                .Where(d => d.StartDate <= today && d.EndDate >= today)
                .ToListAsync();

            // Tính tổng tiền
            decimal basePrice = showtime.Movie.BasePrice;
            decimal totalAmount = selectedSeats.Sum(s => basePrice + (s.SeatType.AdditionalCharge ?? 0));

            ViewBag.Showtime = showtime;
            ViewBag.SelectedSeats = selectedSeats;
            ViewBag.Discounts = discounts;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.SeatIds = seatIds;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcessBooking(int[] seatIds, int showtimeId, int? userId, int? discountId)
        {
            if (seatIds == null || seatIds.Length == 0)
            {
                return RedirectToAction("SelectSeats", new { showtimeId });
            }

            // Lấy thông tin ghế và suất chiếu
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
            {
                return NotFound();
            }

            // Lấy thông tin ghế đã chọn
            var selectedSeats = await _context.Seats
                .Include(s => s.SeatType)
                .Where(s => seatIds.Contains(s.SeatId))
                .ToListAsync();

            // Tính tổng tiền
            decimal basePrice = showtime.Movie.BasePrice;
            decimal totalAmount = selectedSeats.Sum(s => basePrice + (s.SeatType.AdditionalCharge ?? 0));

            // Xử lý khuyến mãi nếu có
            if (discountId.HasValue)
            {
                var discount = await _context.Discounts.FindAsync(discountId.Value);
                if (discount != null)
                {
                    totalAmount = totalAmount * (1 - discount.DiscountValue / 100);
                }
            }

            // Tạo booking mới
            var booking = new Booking
            {
                UserId = userId ?? 1, // Mặc định là người dùng ID 1 nếu không có
                BookingDate = DateTime.Now,
                TotalAmount = totalAmount,
                DiscountId = discountId,
                PaymentDueDate = DateTime.Now.AddMinutes(15),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Tạo trạng thái booking (Đã tạo)
            var bookingStatus = new BookingBookingStatus
            {
                BookingId = booking.BookingId,
                BookingStatusId = 1 // Giả sử 1 là trạng thái "Đã tạo"
            };

            _context.BookingBookingStatuses.Add(bookingStatus);

            // Tạo tickets cho từng ghế
            foreach (var seatId in seatIds)
            {
                var ticket = new Ticket
                {
                    BookingId = booking.BookingId,
                    SeatId = seatId,
                    ShowtimeId = showtimeId,
                    Price = basePrice + (selectedSeats.First(s => s.SeatId == seatId).SeatType.AdditionalCharge ?? 0),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Tickets.Add(ticket);

                // Thêm trạng thái vé (Đã đặt)
                var ticketStatus = new TicketTicketStatus
                {
                    TicketId = ticket.TicketId,
                    TicketStatusId = 1 // Giả sử 1 là trạng thái "Đã đặt"
                };

                _context.TicketTicketStatuses.Add(ticketStatus);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmation", new { bookingId = booking.BookingId });
        }

        public async Task<IActionResult> Confirmation(int bookingId)
        {
            // Lấy thông tin booking
            var booking = await _context.Bookings
                .Include(b => b.Discount)
                .Include(b => b.User)
                .Include(b => b.BookingBookingStatuses)
                    .ThenInclude(bbs => bbs.BookingStatus)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                        .ThenInclude(s => s.SeatType)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 