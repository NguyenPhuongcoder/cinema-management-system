using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Services;
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
        private readonly IMovieService _movieService;
        private readonly ITicketService _ticketService;
        private readonly IShowtimeService _showtimeService;
        private readonly ISeatService _seatService;
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;
        private readonly IPaymentService _paymentService;
        private readonly IDiscountService _discountService;

        public StaffBookingController(
            ILogger<StaffBookingController> logger,
            CinemaDbContext context,
            IMovieService movieService,
            ITicketService ticketService,
            IShowtimeService showtimeService,
            ISeatService seatService,
            IUserService userService,
            IBookingService bookingService,
            IPaymentService paymentService,
            IDiscountService discountService)
        {
            _logger = logger;
            _context = context;
            _movieService = movieService;
            _ticketService = ticketService;
            _showtimeService = showtimeService;
            _seatService = seatService;
            _userService = userService;
            _bookingService = bookingService;
            _paymentService = paymentService;
            _discountService = discountService;
        }

        public IActionResult Index()
        {
            var movies = _showtimeService.GetAllMovies();
            return View(movies);
        }

        public IActionResult SelectMovie()
        {
            try
            {
                var movies = _showtimeService.GetAllMovies();
                return View(movies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách phim");
                return View(new List<Movie>());
            }
        }

        public IActionResult SelectShowtime(int movieId)
        {
            try
            {
                var showtimes = _showtimeService.GetShowtimesByMovie(movieId);
                var movie = _showtimeService.GetMovieById(movieId);
                ViewBag.Movie = movie;
                return View(showtimes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách suất chiếu");
                return RedirectToAction("SelectMovie");
            }
        }

        public IActionResult SelectSeats(int showtimeId)
        {
            try
            {
                var showtime = _showtimeService.GetShowtimeById(showtimeId);
                if (showtime == null)
                {
                    return RedirectToAction("SelectShowtime", new { movieId = showtime?.Movie?.MovieId });
                }

                var seats = _seatService.GetSeatsByRoom(showtime.RoomId);
                var bookedSeats = _ticketService.GetBookedSeats(showtimeId);

                ViewBag.Showtime = showtime;
                ViewBag.RoomSeats = seats;
                ViewBag.BookedSeats = bookedSeats;
                ViewBag.BasePrice = showtime.Movie?.BasePrice ?? 0m;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin ghế");
                return RedirectToAction("SelectShowtime", new { movieId = 0 });
            }
        }

        public IActionResult Checkout(int showtimeId, List<int> seatIds)
        {
            try
            {
                var showtime = _showtimeService.GetShowtimeById(showtimeId);
                if (showtime == null)
                {
                    return RedirectToAction("SelectShowtime", new { movieId = showtime?.Movie?.MovieId });
                }

                var seats = _seatService.GetSeatsByIds(seatIds);
                if (seats == null || !seats.Any() || seats.Count() != seatIds.Count)
                {
                    return RedirectToAction("SelectSeats", new { showtimeId });
                }

                var basePrice = showtime.Movie?.BasePrice ?? 0m;
                var totalAmount = seats.Sum(s => 
                    s.SeatType?.TypeName.Contains("VIP") == true ? basePrice + 30000m :
                    s.SeatType?.TypeName.Contains("Đôi") == true ? basePrice * 2m : basePrice
                );

                ViewBag.Showtime = showtime;
                ViewBag.SeatIds = seatIds;
                ViewBag.SelectedSeats = seats;
                ViewBag.TotalAmount = totalAmount;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý thanh toán");
                return RedirectToAction("SelectSeats", new { showtimeId });
            }
        }

        [HttpGet]
        public IActionResult CheckDiscount(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Json(new { success = false, message = "Mã giảm giá không được để trống" });
            }

            var discount = _discountService.GetDiscountByCode(code);
            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không tồn tại" });
            }

            var now = DateTime.Now;
            if (discount.StartDate > now || discount.EndDate < now)
            {
                return Json(new { success = false, message = "Mã giảm giá đã hết hạn" });
            }

            if (!discount.IsActive)
            {
                return Json(new { success = false, message = "Mã giảm giá không còn hiệu lực" });
            }

            return Json(new { success = true, discount = discount });
        }

        [HttpGet]
        public IActionResult CheckMember(int userId)
        {
            try
            {
                var user = _userService.GetUserById(userId);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin thành viên" });
                }

                return Json(new { 
                    success = true, 
                    user = new { 
                        fullName = user.FullName,
                        phone = user.Phone,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi kiểm tra thông tin thành viên");
                return Json(new { success = false, message = "Có lỗi xảy ra khi kiểm tra thông tin thành viên" });
            }
        }

        [HttpPost]
        public IActionResult ProcessBooking(int showtimeId, List<int> seatIds, string fullName, string phone, string email, int paymentMethodId, int? userId = null, int? discountId = null, string note = null)
        {
            try
            {
                // Kiểm tra thông tin đầu vào
                if (showtimeId <= 0 || seatIds == null || !seatIds.Any())
                {
                    return RedirectToAction("SelectSeats", new { showtimeId });
                }

                // Lấy thông tin suất chiếu
                var showtime = _showtimeService.GetShowtimeById(showtimeId);
                if (showtime == null)
                {
                    return RedirectToAction("SelectShowtime", new { movieId = showtime?.Movie?.MovieId });
                }

                // Kiểm tra ghế đã được đặt chưa
                var bookedSeats = _ticketService.GetBookedSeats(showtimeId);
                if (seatIds.Any(id => bookedSeats.Contains(id)))
                {
                    return RedirectToAction("SelectSeats", new { showtimeId });
                }

                // Lấy thông tin ghế
                var seats = _seatService.GetSeatsByIds(seatIds);
                if (seats == null || !seats.Any() || seats.Count() != seatIds.Count)
                {
                    return RedirectToAction("SelectSeats", new { showtimeId });
                }

                // Tính toán giá vé
                var basePrice = showtime.Movie?.BasePrice ?? 0m;
                var totalAmount = seats.Sum(s => 
                    s.SeatType?.TypeName.Contains("VIP") == true ? basePrice + 30000m :
                    s.SeatType?.TypeName.Contains("Đôi") == true ? basePrice * 2m : basePrice
                );

                // Áp dụng giảm giá nếu có
                decimal discountAmount = 0m;
                if (discountId.HasValue)
                {
                    var discount = _discountService.GetDiscountById(discountId.Value);
                    if (discount != null && discount.IsActive)
                    {
                        discountAmount = totalAmount * discount.DiscountValue / 100m;
                    }
                }

                var finalAmount = totalAmount - discountAmount;

                // Tạo hoặc cập nhật thông tin người dùng
                var user = userId.HasValue ? _userService.GetUserById(userId.Value) : null;
                if (user == null)
                {
                    user = new User
                    {
                        Username = email,
                        Password = "default123", // Set a default password
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    user.FullName = fullName; // Set after creation
                    user.Phone = phone;
                    user.Email = email;
                    _userService.CreateUser(user);
                }
                else
                {
                    user.Username = email;
                    user.FullName = fullName; // Set after getting user
                    user.Phone = phone;
                    user.Email = email;
                    user.UpdatedAt = DateTime.Now;
                    _userService.UpdateUser(user);
                }

                // Tạo đơn đặt vé
                var booking = new Booking
                {
                    UserId = user.UserId,
                    BookingDate = DateTime.Now,
                    TotalAmount = finalAmount,
                    DiscountId = discountId,
                    Note = note,
                    CreatedAt = DateTime.Now
                };
                _bookingService.CreateBookingAsync(booking);

                // Tạo trạng thái đặt vé
                var bookingStatus = new BookingBookingStatus
                {
                    BookingId = booking.BookingId,
                    BookingStatusId = 1, // Trạng thái đã xác nhận
                    StatusDate = DateTime.Now
                };
                _context.BookingBookingStatuses.Add(bookingStatus);

                // Tạo vé cho từng ghế
                foreach (var seat in seats)
                {
                    var price = seat.SeatType?.TypeName.Contains("VIP") == true ? basePrice + 30000m :
                               seat.SeatType?.TypeName.Contains("Đôi") == true ? basePrice * 2m : basePrice;

                    var ticket = new Ticket
                    {
                        BookingId = booking.BookingId,
                        ShowtimeId = showtimeId,
                        SeatId = seat.SeatId,
                        Price = price,
                        TicketCode = GenerateTicketCode(),
                        TicketStatus = "confirmed",
                        CreatedAt = DateTime.Now
                    };
                    _ticketService.CreateTicket(ticket);
                }

                // Tạo thanh toán
                var payment = new Payment
                {
                    BookingId = booking.BookingId,
                    PaymentMethodId = paymentMethodId,
                    Amount = finalAmount,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "completed",
                    CreatedAt = DateTime.Now
                };
                _paymentService.CreatePayment(payment);

                _context.SaveChanges();

                return RedirectToAction("Confirmation", new { bookingId = booking.BookingId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý đặt vé");
                return RedirectToAction("Checkout", new { showtimeId, seatIds });
            }
        }

        private string GenerateTicketCode()
        {
            return "TKT" + DateTime.Now.ToString("yyyyMMddHHmmss") + new Random().Next(1000, 9999);
        }

        public IActionResult Confirmation(int bookingId)
        {
            try
            {
                var booking = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
                            .ThenInclude(s => s.Movie)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                    .FirstOrDefault(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    return RedirectToAction("Index");
                }

                return View(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xem thông tin đặt vé");
                return RedirectToAction("Index");
            }
        }

        public IActionResult PrintTicket(int ticketId)
        {
            try
            {
                var ticket = _ticketService.GetTicketById(ticketId);
                if (ticket == null)
                {
                    return RedirectToAction("Index");
                }

                return View("_TicketDetails", ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi in vé");
                return RedirectToAction("Index");
            }
        }

        public IActionResult SendTicketEmail(int ticketId)
        {
            try
            {
                var ticket = _ticketService.GetTicketById(ticketId);
                if (ticket == null)
                {
                    return RedirectToAction("Index");
                }

                // TODO: Implement email sending logic
                return RedirectToAction("Confirmation", new { bookingId = ticket.BookingId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi email vé");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ManageTickets(
            string ticketCode = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1)
        {
            try
            {
                // Lấy danh sách phim cho dropdown
                ViewBag.Movies = new SelectList(await _movieService.GetAllMoviesAsync(), "MovieId", "Title");

                // Lưu các tham số tìm kiếm vào ViewBag
                ViewBag.TicketCode = ticketCode;
                ViewBag.CustomerPhone = customerPhone;
                ViewBag.MovieId = movieId;
                ViewBag.Status = status;
                ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
                ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

                // Xây dựng query string cho phân trang
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(ticketCode)) queryParams.Add($"ticketCode={ticketCode}");
                if (!string.IsNullOrEmpty(customerPhone)) queryParams.Add($"customerPhone={customerPhone}");
                if (movieId.HasValue) queryParams.Add($"movieId={movieId}");
                if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
                if (fromDate.HasValue) queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
                if (toDate.HasValue) queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");
                ViewBag.QueryString = string.Join("&", queryParams);

                // Lấy danh sách vé theo điều kiện tìm kiếm
                var tickets = await _ticketService.GetTicketsAsync(
                    ticketCode,
                    customerPhone,
                    movieId,
                    status,
                    fromDate,
                    toDate,
                    page,
                    10 // Số vé mỗi trang
                );

                // Tính toán thông tin phân trang
                var totalRecords = await _ticketService.CountTicketsAsync(
                    ticketCode,
                    customerPhone,
                    movieId,
                    status,
                    fromDate,
                    toDate
                );
                var totalPages = (int)Math.Ceiling(totalRecords / 10.0);
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalRecords = totalRecords;
                ViewBag.StartRecord = (page - 1) * 10 + 1;
                ViewBag.EndRecord = Math.Min(page * 10, totalRecords);

                ViewBag.Tickets = tickets;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ManageTickets action");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketDetails(int ticketId)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                {
                    return NotFound();
                }
                return PartialView("_TicketDetails", ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTicketDetails action");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi lấy thông tin vé" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelTicket(int ticketId)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(ticketId);
                if (ticket == null)
                {
                    return NotFound();
                }

                ticket.TicketStatus = "cancelled";
                ticket.UpdatedAt = DateTime.Now;
                await _ticketService.UpdateTicketAsync(ticket);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CancelTicket action");
                return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi hủy vé" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 