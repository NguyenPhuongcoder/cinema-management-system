using Microsoft.AspNetCore.Mvc;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.ViewModels;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CinemaDbContext _context;
        public CustomerController(CinemaDbContext context)
        {
            _context = context;
        }
    public IActionResult Index()
    {
         var banners = new List<string>
        {
        "/images/2048x682_1746503337124.jpg",
        "/images/chong-gai-1_1744949649177.jpg",
        "/images/doraemon-the-movie-nobitas-art-world-tales-3_1746800280056.jpg",
        "/images/gravity-1_1747208790653.jpg",
        "/images/tham-tu-kien-1_1745832794637.jpg",
        "/images/thunderbolts-2048_1745395976662.jpg"
        };
        var allMovies = _context.Movies.ToList();
        var provinces = _context.Provinces.ToList();
        var vm = new HomePageViewModel
        {
            Banners = banners,
            Movies = allMovies,
            Provinces = provinces
        };
        return View(vm);
    }
    }
}
