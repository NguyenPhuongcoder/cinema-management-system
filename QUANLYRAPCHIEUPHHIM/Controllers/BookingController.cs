using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{

    [Authorize(Roles = "customer")]
    public class BookingController : Controller
    {
        private readonly CinemaDbcontext _context;
        public BookingController(CinemaDbcontext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            TempData.Remove("DiscountId");

            // --- 1. Lấy danh sách tỉnh và tỉnh được chọn ---
            var provinces = _context.Provinces.ToList();
            var selectedProvinceId = TempData["ProvinceId"] as int? ?? provinces.FirstOrDefault()?.ProvinceId;

            // --- 2. Truy vấn tất cả các phần phụ thuộc ---
            var cityIds = _context.Cities
                .Where(city => city.ProvinceId == selectedProvinceId)
                .Select(city => city.CityId)
                .ToList();

            var addressIds = _context.Addresses
                .Where(addr => cityIds.Contains(addr.CityId))
                .Select(addr => addr.AddressId)
                .ToList();

            var cinemasInProvince = _context.Cinemas
                .Where(c => addressIds.Contains(c.AddressId))
                .ToList();
            var cinemaIds = cinemasInProvince.Select(c => c.CinemaId).ToList();

            var rooms = _context.Rooms
                .Where(r => cinemaIds.Contains(r.CinemaId))
                .ToList();
            var roomIds = rooms.Select(r => r.RoomId).ToList();

            // --- 3. Lấy danh sách phim có suất chiếu trong các room này ---
            var movies = _context.Movies
                .Where(m => m.Showtimes.Any(s => roomIds.Contains(s.RoomId)))
                .ToList();
            var selectedMovieId = TempData["MovieId"] as int? ?? movies.FirstOrDefault()?.MovieId;

            // --- 4. Lấy tất cả suất chiếu của phim đã chọn ---
            var allShowtimes = new List<Showtime>();
            if (selectedMovieId != null)
            {
                allShowtimes = _context.Showtimes
                    .Where(s => s.MovieId == selectedMovieId && roomIds.Contains(s.RoomId))
                    .ToList();
            }

            // --- 5. Lấy các ngày có suất chiếu ---
            var availableDates = allShowtimes
                .Select(s => s.StartTime.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            DateTime? selectedDate = null;
            if (TempData["SelectedDate"] != null)
            {
                if (DateTime.TryParseExact(TempData["SelectedDate"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
                    selectedDate = parsed;
            }
            selectedDate ??= availableDates.FirstOrDefault();

            // --- 6. Lấy các rạp có suất chiếu vào ngày đã chọn ---
            var roomIdWithShowInDate = allShowtimes
                .Where(s => s.StartTime.Date == selectedDate)
                .Select(s => s.RoomId)
                .Distinct()
                .ToList();

            var cinemaIdWithShow = rooms
                .Where(r => roomIdWithShowInDate.Contains(r.RoomId))
                .Select(r => r.CinemaId)
                .Distinct()
                .ToList();

            var cinemas = cinemasInProvince
                .Where(c => cinemaIdWithShow.Contains(c.CinemaId))
                .ToList();

            // --- 7. Chọn rạp (nếu có) ---
            var selectedCinemaId = TempData["CinemaId"] as int?;

            // --- 8. Lọc danh sách suất chiếu phù hợp ---
            List<Showtime> showtimes;
            if (selectedCinemaId.HasValue)
            {
                var roomIdsOfSelectedCinema = rooms
                    .Where(r => r.CinemaId == selectedCinemaId)
                    .Select(r => r.RoomId)
                    .ToList();

                showtimes = allShowtimes
                    .Where(s => s.StartTime.Date == selectedDate && roomIdsOfSelectedCinema.Contains(s.RoomId))
                    .OrderBy(s => s.StartTime)
                    .ToList();
            }
            else
            {
                showtimes = allShowtimes
                    .Where(s => s.StartTime.Date == selectedDate)
                    .OrderBy(s => s.StartTime)
                    .ToList();
            }

            // --- 9. Gán vào ViewModel ---
            var vm = new BookingPageViewModel
            {
                Provinces = provinces,
                SelectedProvinceId = selectedProvinceId,
                Movies = movies,
                SelectedMovieId = selectedMovieId,
                ShowtimeGroups = new List<ShowtimeGroupViewModel>(),
                AvailableDates = availableDates,
                SelectedDate = selectedDate,
                Cinemas = cinemas,
                SelectedCinemaId = selectedCinemaId,
                Showtimes = showtimes,
                SelectedShowtimeId = TempData["ShowtimeId"] as int?
            };

            // --- 10. Lưu lại TempData ---
            TempData["ProvinceId"] = selectedProvinceId;
            TempData["MovieId"] = selectedMovieId;
            TempData["SelectedDate"] = selectedDate?.ToString("yyyy-MM-dd");
            TempData["CinemaId"] = selectedCinemaId;

            return View("Booking", vm);
        }


        [HttpPost]
        public IActionResult Index(BookingPageViewModel model, string submitButton)
        {
            // Lưu các giá trị vào TempData
            TempData["ProvinceId"] = model.SelectedProvinceId;
            TempData["MovieId"] = model.SelectedMovieId;
            TempData["SelectedDate"] = model.SelectedDate?.ToString("yyyy-MM-dd");
            TempData["CinemaId"] = model.SelectedCinemaId;
            TempData["ShowtimeId"] = model.SelectedShowtimeId;

            // Nếu bấm nút Tiếp tục và đã chọn suất chiếu
            if (submitButton == "continue" && model.SelectedShowtimeId.HasValue)
            {
                return RedirectToAction("SelectSeat");
            }

            return RedirectToAction("Index");
        }

        public IActionResult SelectSeat()

        {

            var selectedShowtimeId = TempData["ShowtimeId"] as int?;
            if (!selectedShowtimeId.HasValue)
            {
                return RedirectToAction("Index");
            }

            // Giữ lại giá trị ShowtimeId cho các request tiếp theo
            TempData.Keep("ShowtimeId");

            var showtime = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ThenInclude(r => r.Cinema)
                .FirstOrDefault(s => s.ShowtimeId == selectedShowtimeId);

            if (showtime == null) return NotFound();

            // Lấy danh sách ghế và trạng thái đã bán
            var seats = _context.Seats
                .Include(s => s.SeatType)
                .Where(s => s.RoomId == showtime.Room.RoomId)
                .ToList();

            var soldSeatIds = _context.Tickets
                .Where(t => t.ShowtimeId == selectedShowtimeId)
                .Select(t => t.SeatId)
                .ToList();

            var seatStatus = seats.Select(s => new SeatStatusViewModel
            {
                SeatId = s.SeatId,
                Row = s.RowLetter,
                Number = s.SeatNumber,
                IsSold = soldSeatIds.Contains(s.SeatId),
                Type = s.SeatType.TypeName,
                Price = (decimal)(showtime.PriceModifier + (s.SeatType.AdditionalCharge ?? 0))
            }).ToList();

            var vm = new BookingSeatViewModel
            {
                Movie = showtime.Movie,
                Cinema = showtime.Room.Cinema,
                Showtime = showtime,
                Room = showtime.Room,
                Seats = seatStatus
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Payment(List<int> selectedSeatIds)
        {
            var selectedShowtimeId = TempData["ShowtimeId"] as int?;
            if (!selectedShowtimeId.HasValue)
            {
                return RedirectToAction("Index");
            }

            // Giữ lại ShowtimeId cho các request tiếp theo
            TempData.Keep("ShowtimeId");

            // Nếu có selectedSeatIds từ form post, lưu vào TempData
            if (selectedSeatIds != null && selectedSeatIds.Any())
            {
                TempData["SelectedSeatIds"] = JsonSerializer.Serialize(selectedSeatIds);
            }
            // Nếu không có từ form, thử lấy từ TempData
            else if (TempData["SelectedSeatIds"] != null)
            {
                selectedSeatIds = JsonSerializer.Deserialize<List<int>>(TempData["SelectedSeatIds"].ToString());
                // Giữ lại cho request tiếp theo
                TempData.Keep("SelectedSeatIds");
            }

            if (selectedSeatIds == null || !selectedSeatIds.Any())
            {
                return RedirectToAction("SelectSeat");
            }

            var showtime = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .ThenInclude(r => r.Cinema)
                .FirstOrDefault(s => s.ShowtimeId == selectedShowtimeId);

            if (showtime == null) return NotFound();

            var selectedSeats = _context.Seats
                .Include(s => s.SeatType)
                .Where(s => selectedSeatIds.Contains(s.SeatId))
                .ToList();

            // Tính tổng tiền dựa trên loại ghế
            var totalAmount = selectedSeats.Sum(s => 
                showtime.PriceModifier + (s.SeatType.AdditionalCharge ?? 0));

            var paymentMethods = new List<string>
            {
                "Thanh toán bằng tiền mặt",
                "Thanh toán bằng thẻ ngân hàng",
                "Ví điện tử MoMo",
                "ZaloPay"
            };

            var viewModel = new BookingPaymentViewModel
            {
                Movie = showtime.Movie,
                Cinema = showtime.Room.Cinema,
                Showtime = showtime,
                Room = showtime.Room,
                SelectedSeats = selectedSeats,
                TotalPrice = (decimal)totalAmount,
                PaymentMethods = paymentMethods
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult Payment()
        {
            var selectedSeatIds = JsonSerializer.Deserialize<List<int>>(TempData["SelectedSeatIds"]?.ToString() ?? "[]");

            return Payment(selectedSeatIds); // Gọi lại hàm POST nội bộ
        }

        [HttpPost]
        [Authorize]
        public IActionResult ApplyDiscount(string promoCode)
        {
            // Lấy thông tin từ TempData
            var selectedShowtimeId = TempData["ShowtimeId"] as int?;
            if (!selectedShowtimeId.HasValue)
            {
                return RedirectToAction("Index");
            }

            // Giữ lại các giá trị TempData cho request tiếp theo
            TempData.Keep("ShowtimeId");
            TempData.Keep("SelectedSeatIds");

            var discount = _context.Discounts
                .FirstOrDefault(d => d.CouponCode == promoCode && d.IsActive == true);

            if (discount == null)
            {
                TempData["Error"] = "Mã giảm giá không hợp lệ!";
                return RedirectToAction("Payment");
            }

            // Kiểm tra thời hạn
            var today = DateOnly.FromDateTime(DateTime.Now);
            if (today < discount.StartDate || today > discount.EndDate)
            {
                TempData["Error"] = "Mã giảm giá đã hết hạn!";
                return RedirectToAction("Payment");
            }

            // Kiểm tra giới hạn sử dụng
            if (discount.UsageLimit.HasValue)
            {
                var usageCount = _context.Bookings.Count(b => b.DiscountId == discount.DiscountId);
                if (usageCount >= discount.UsageLimit.Value)
                {
                    TempData["Error"] = "Mã giảm giá đã hết lượt sử dụng!";
                    return RedirectToAction("Payment");
                }
            }

            // Lưu mã giảm giá vào TempData để sử dụng khi thanh toán
            TempData["DiscountId"] = discount.DiscountId;
            TempData["DiscountValue"] = discount.DiscountValue.ToString();
            TempData["Success"] = $"Áp dụng mã giảm giá thành công! Giảm {discount.DiscountValue}%";

            return RedirectToAction("Payment");
        }

        [HttpPost]
        [Authorize]
        public IActionResult ProcessPayment(string selectedPaymentMethod)
        {
            if (string.IsNullOrEmpty(selectedPaymentMethod))
            {
                TempData["Error"] = "Vui lòng chọn phương thức thanh toán";
                return RedirectToAction("Payment");
            }

            var selectedSeatIds = JsonSerializer.Deserialize<List<int>>(TempData["SelectedSeatIds"].ToString());
            var selectedShowtimeId = (int)TempData["ShowtimeId"];

            int userId = 0;
            if (User.Identity.IsAuthenticated)
            {
                int.TryParse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value, out userId);
            }

            var showtime = _context.Showtimes.FirstOrDefault(s => s.ShowtimeId == selectedShowtimeId);
            var selectedSeats = _context.Seats
                .Include(s => s.SeatType)
                .Where(s => selectedSeatIds.Contains(s.SeatId))
                .ToList();

            // Tính tổng tiền dựa trên loại ghế
            var totalAmount = selectedSeats.Sum(s => 
                showtime.PriceModifier + (s.SeatType.AdditionalCharge ?? 0));

            int? discountId = null;
            // Áp dụng giảm giá nếu có
            if (TempData.ContainsKey("DiscountId"))
            {
                 discountId = int.Parse(TempData["DiscountId"].ToString());
                 var discount = _context.Discounts.Find(discountId);

                var discountValue = discount.DiscountValue;
                discount.UsageLimit = discount.UsageLimit - 1;
                _context.Discounts.Update(discount);
                TempData.Remove("DiscountId");
                totalAmount = totalAmount -  (int)discountValue / 100;
            }

            // Create booking
            var booking = new Booking
            {
                UserId = userId,
                BookingDate = DateTime.Now,
                TotalAmount = (decimal)totalAmount,
                DiscountId = discountId
            };
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // Create tickets
            foreach (var seatId in selectedSeatIds)
            {
                // Tìm thông tin ghế để lấy loại ghế
                var seat = selectedSeats.FirstOrDefault(s => s.SeatId == seatId);

                var ticket = new Ticket
                {
                    BookingId = booking.BookingId,
                    ShowtimeId = selectedShowtimeId,
                    SeatId = seatId,
                    // Giá = Giá showtime + Phụ phí loại ghế
                    Price = (decimal)(showtime.PriceModifier + (seat?.SeatType?.AdditionalCharge ?? 0)),
                    TicketStatus = "Đã đặt",
                    TicketCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                    CreatedAt = DateTime.Now
                };
                _context.Tickets.Add(ticket);
            }


            // Create payment record
            var payment = new Payment
            {
                BookingId = booking.BookingId,
                PaymentMethodId = 1, // You should map this based on the selected payment method
                Amount = (decimal)totalAmount,
                PaymentDate = DateTime.Now,
                PaymentStatus = "Completed",
                CreatedAt = DateTime.Now
            };
            _context.Payments.Add(payment);

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