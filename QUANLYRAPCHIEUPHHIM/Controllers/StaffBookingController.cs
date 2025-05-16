using Microsoft.AspNetCore.Mvc;
using QUANLYRAPCHIEUPHHIM.Models;
using System.Diagnostics;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class StaffBookingController : Controller
    {
        private readonly ILogger<StaffBookingController> _logger;

        public StaffBookingController(ILogger<StaffBookingController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SelectMovie()
        {
            // Phần này sẽ hiển thị danh sách phim đang chiếu để nhân viên chọn
            return View();
        }

        public IActionResult SelectShowtime(int movieId)
        {
            // Hiển thị danh sách suất chiếu của phim đã chọn
            return View();
        }

        public IActionResult SelectSeats(int showtimeId)
        {
            // Hiển thị sơ đồ ghế để chọn
            return View();
        }

        public IActionResult Checkout(int[] seatIds, int showtimeId)
        {
            // Trang thanh toán và xác nhận đặt vé
            return View();
        }

        public IActionResult Confirmation(int bookingId)
        {
            // Trang xác nhận đặt vé thành công
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
} 