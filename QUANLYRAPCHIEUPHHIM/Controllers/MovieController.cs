using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using QUANLYRAPCHIEUPHHIM.ViewModels;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class MovieController : Controller
    {
        private readonly CinemaDbcontext _context;
        private readonly ILogger<MovieController> _logger;

        public MovieController(CinemaDbcontext context, ILogger<MovieController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Movies(string type)
        {
            try
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            List<Movie> movies = new();
            string title = "Danh sách phim";

            switch ((type ?? "nowshowing").ToLower())
            {
                case "comingsoon":
                    movies = _context.Movies
                        .AsNoTracking()
                        .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value > today)
                        .OrderBy(m => m.ReleaseDate)
                        .ToList();
                    title = "Phim Sắp Chiếu";
                    break;
                case "imax":
                        var imaxFormat = _context.RoomFormats.AsNoTracking()
                            .FirstOrDefault(f => f.FormatName == "IMAX");
                    if (imaxFormat != null)
                    {
                        movies = (from m in _context.Movies.AsNoTracking()
                                    join mf in _context.MovieFormats.AsNoTracking() 
                                    on m.MovieId equals mf.MovieId
                                  where mf.FormatId == imaxFormat.FormatId
                                  select m).ToList();
                    }
                    title = "Phim IMAX";
                    break;
                default:
                    movies = _context.Movies
                        .AsNoTracking()
                        .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value <= today)
                        .OrderByDescending(m => m.ReleaseDate)
                        .ToList();
                    title = "Phim Đang Chiếu";
                    break;
            }

            ViewBag.Title = title;
            ViewBag.Type = (type ?? "nowshowing").ToLower();
            return View("Movies", movies);
        }
            catch (Exception ex)
        {
                _logger.LogError(ex, "Error in Movies action");
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
                    {
            try
            {
                if (id <= 0)
                {
                    ModelState.AddModelError("", "ID phim không hợp lệ");
                    return View("Error");
        }

                var movie = await _context.Movies
                    .Include(m => m.MovieGenres)
                        .ThenInclude(g => g.Genre)
                    .Include(m => m.MovieCasts)
                        .ThenInclude(mc => mc.Person)
                    .Include(m => m.MovieCasts)
                        .ThenInclude(mc => mc.MovieCastRoleTypes)
                            .ThenInclude(rt => rt.RoleType)
                    .FirstOrDefaultAsync(m => m.MovieId == id);

                if (movie == null)
                {
                    _logger.LogWarning($"Movie with ID {id} not found");
                    return NotFound();
                }

            var directors = movie.MovieCasts
                .Where(mc => mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director"))
                .Select(mc => mc.Person.FullName)
                .ToList();

            var actors = movie.MovieCasts
                .Where(mc => mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Actor"))
                .Select(mc => mc.Person.FullName)
                .ToList();

            ViewBag.Directors = directors;
            ViewBag.Actors = actors;

                return View(movie);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Details action for movie ID {id}");
                return View("Error");
            }
        }

        public IActionResult Library(string genre, int? year, string status, int page = 1, int pageSize = 8)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 8;

                var genres = _context.Genres.OrderBy(g => g.GenreName).ToList();
                var years = _context.Movies
                    .Where(m => m.ReleaseDate.HasValue)
                    .Select(m => m.ReleaseDate.Value.Year)
                    .Distinct()
                    .OrderByDescending(y => y)
                    .ToList();

                var movies = _context.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(g => g.Genre)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(genre))
                    {
                        movies = movies.Where(m => m.MovieGenres.Any(g => g.Genre.GenreName == genre));
                }

                if (year.HasValue)
                    {
                        movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year == year);
                }

                if (!string.IsNullOrEmpty(status))
                    {
                        var today = DateOnly.FromDateTime(DateTime.Today);
                        if (status == "nowshowing")
                            movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value <= today);
                    else if (status == "comingsoon")
                            movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value > today);
                    }

                // Default sorting by release date
                        movies = movies.OrderByDescending(m => m.ReleaseDate);

                var totalItems = movies.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var pagedMovies = movies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                var vm = new MovieLibraryViewModel
                {
                    Movies = pagedMovies,
                    Genres = genres,
                    Years = years,
                    SelectedGenre = genre,
                    SelectedYear = year,
                    SelectedStatus = status,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalItems = totalItems
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Library action");
                ModelState.AddModelError("", "Có lỗi xảy ra khi tải danh sách phim");
                return View(new MovieLibraryViewModel());
            }
        }

        public IActionResult LibraryAjax(string genre, int? year, string status, int page = 1, int pageSize = 8, string partial = "")
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 8;

                var movies = _context.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(g => g.Genre)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(genre))
                    {
                        movies = movies.Where(m => m.MovieGenres.Any(g => g.Genre.GenreName == genre));
                }

                if (year.HasValue)
                    {
                        movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value.Year == year);
                }

                if (!string.IsNullOrEmpty(status))
                    {
                        var today = DateOnly.FromDateTime(DateTime.Today);
                        if (status == "nowshowing")
                            movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value <= today);
                    else if (status == "comingsoon")
                            movies = movies.Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value > today);
                    }

                // Default sorting by release date
                        movies = movies.OrderByDescending(m => m.ReleaseDate);

                var totalItems = movies.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var pagedMovies = movies.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                if (partial == "pagination")
                {
                    var paginationModel = new
                    {
                        CurrentPage = page,
                        TotalPages = totalPages,
                        PageSize = pageSize,
                        TotalItems = totalItems
                    };
                    return PartialView("_Pagination", paginationModel);
                }

                return PartialView("_MovieLibraryList", pagedMovies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LibraryAjax action");
                return Json(new { error = "Có lỗi xảy ra khi tải danh sách phim" });
            }
        }
    }
}
