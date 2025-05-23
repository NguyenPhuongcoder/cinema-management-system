using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.ViewModels;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class HomeController : Controller
    {
        private readonly CinemaDbcontext _context;

        public HomeController(CinemaDbcontext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Lấy banners với AsNoTracking và giới hạn 5 bản ghi
            var banners = _context.Movies
                .AsNoTracking()
                .Where(m =>
                    !string.IsNullOrEmpty(m.PanelUrl) &&
                    m.ReleaseDate.HasValue &&
                    m.ReleaseDate.Value <= today
                )
                .OrderByDescending(m => m.ReleaseDate)
                .Take(10)
                .Select(m => m.PanelUrl!)
                .ToList();

            // Phim đang chiếu, lấy tối đa 10 phim, AsNoTracking
            var nowShowing = _context.Movies
                .AsNoTracking()
                .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value <= today)
                .OrderByDescending(m => m.ReleaseDate)
                .Take(10)
                .ToList();

            // Phim sắp chiếu, lấy tối đa 10 phim, AsNoTracking
            var comingSoon = _context.Movies
                .AsNoTracking()
                .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value > today)
                .OrderBy(m => m.ReleaseDate)
                .Take(10) // Giới hạn số lượng phim sắp chiếu
                .ToList();


            // Lấy định dạng IMAX
            var imaxFormat = _context.RoomFormats
                .AsNoTracking()
                .FirstOrDefault(f => f.FormatName == "IMAX");

            List<Movie> imaxMovies = new();

            if (imaxFormat != null)
            {
                // Truy vấn duy nhất lấy phim IMAX kèm giới hạn
                imaxMovies = (from m in _context.Movies.AsNoTracking()
                              join mf in _context.MovieFormats.AsNoTracking()
                              on m.MovieId equals mf.MovieId
                              where mf.FormatId == imaxFormat.FormatId
                              select m)
                              .Take(8) // Giới hạn số phim IMAX
                              .ToList();
            }

            var vm = new HomePageViewModel
            {
                NowShowing = nowShowing,
                ComingSoon = comingSoon,
                ImaxMovies = imaxMovies,
                Banners = banners
            };

            return View(vm);
        }


    }
}
