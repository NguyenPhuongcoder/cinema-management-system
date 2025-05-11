using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.Data;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly CinemaDbContext _context;

        public AdminController(CinemaDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            // Lấy thống kê cơ bản
            var totalMovies = _context.Movies.Count();
            var totalShows = _context.Showtimes.Count();
            var totalBookings = _context.Bookings.Count();
            var totalUsers = _context.Users.Count();

            ViewBag.TotalMovies = totalMovies;
            ViewBag.TotalShows = totalShows;
            ViewBag.TotalBookings = totalBookings;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
} 