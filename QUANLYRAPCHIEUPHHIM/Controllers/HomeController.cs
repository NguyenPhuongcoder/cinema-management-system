using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Data;
using Microsoft.AspNetCore.Authorization;
using QUANLYRAPCHIEUPHHIM.Services;
using QUANLYRAPCHIEUPHHIM.Models.ViewModels;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CinemaDbContext _context;
        private readonly IMovieService _movieService;
        private readonly ICinemaService _cinemaService;

        public HomeController(
            ILogger<HomeController> logger,
            CinemaDbContext context,
            IMovieService movieService,
            ICinemaService cinemaService)
        {
            _logger = logger;
            _context = context;
            _movieService = movieService;
            _cinemaService = cinemaService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Lấy danh sách phim đang chiếu
                var movies = await _movieService.GetMoviesAsync(
                    page: 1,
                    pageSize: 8
                );

                // Lấy danh sách rạp chiếu
                var cinemas = await _cinemaService.GetCinemasAsync(
                    page: 1,
                    pageSize: 6
                );

                ViewBag.Movies = movies;
                ViewBag.Cinemas = cinemas;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang chủ");
                return View("Error");
            }
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (User.IsInRole("Staff"))
            {
                return RedirectToAction("Index", "StaffTicket");
            }
            else
            {
                return RedirectToAction("Index", "Booking");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
