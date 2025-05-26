using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.ViewModels;

namespace QUANLYRAPCHIEUPHHIM.Controllers
{
    public class DirectorController : Controller
    {
        private readonly CinemaDbcontext _context;
        private readonly ILogger<DirectorController> _logger;

        public DirectorController(CinemaDbcontext context, ILogger<DirectorController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(string nationality, string sort, int page = 1, int pageSize = 12)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 12;

                var nationalities = _context.MoviePeople
                    .Where(p => !string.IsNullOrEmpty(p.Nationality) && 
                           p.MovieCasts.Any(mc => mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")))
                    .Select(p => p.Nationality)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToList();

                var query = _context.MoviePeople
                    .Include(p => p.MovieCasts)
                        .ThenInclude(mc => mc.MovieCastRoleTypes)
                            .ThenInclude(rt => rt.RoleType)
                    .Where(p => p.MovieCasts.Any(mc => mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")))
                    .AsQueryable();

                if (!string.IsNullOrEmpty(nationality))
                {
                    query = query.Where(p => p.Nationality == nationality);
                }

                switch (sort?.ToLower())
                {
                    case "popular":
                        query = query.OrderByDescending(p => p.MovieCasts.Count(mc => 
                            mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")));
                        break;
                    case "name":
                        query = query.OrderBy(p => p.FullName);
                        break;
                    default:
                        query = query.OrderByDescending(p => p.MovieCasts.Count(mc => 
                            mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")));
                        break;
                }

                var totalItems = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var directors = query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

                var viewModel = new DirectorViewModel
                {
                    Directors = directors,
                    Nationalities = nationalities,
                    SelectedNationality = nationality,
                    SelectedSort = sort,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages,
                    TotalItems = totalItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                return View("Error");
            }
        }

        public IActionResult IndexAjax(string nationality, string sort, int page = 1, int pageSize = 12, string partial = "")
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 12;

                var query = _context.MoviePeople
                    .Include(p => p.MovieCasts)
                        .ThenInclude(mc => mc.MovieCastRoleTypes)
                            .ThenInclude(rt => rt.RoleType)
                    .Where(p => p.MovieCasts.Any(mc => mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")))
                    .AsQueryable();

                if (!string.IsNullOrEmpty(nationality))
                {
                    query = query.Where(p => p.Nationality == nationality);
                }

                switch (sort?.ToLower())
                {
                    case "popular":
                        query = query.OrderByDescending(p => p.MovieCasts.Count(mc => 
                            mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")));
                        break;
                    case "name":
                        query = query.OrderBy(p => p.FullName);
                        break;
                    default:
                        query = query.OrderByDescending(p => p.MovieCasts.Count(mc => 
                            mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")));
                        break;
                }

                var totalItems = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var directors = query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

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

                return PartialView("_DirectorList", directors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IndexAjax action");
                return Json(new { error = "Có lỗi xảy ra khi tải danh sách đạo diễn" });
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound();
                }

                var director = await _context.MoviePeople
                    .Include(p => p.MovieCasts)
                        .ThenInclude(mc => mc.Movie)
                    .Include(p => p.MovieCasts)
                        .ThenInclude(mc => mc.MovieCastRoleTypes)
                            .ThenInclude(rt => rt.RoleType)
                    .FirstOrDefaultAsync(p => p.PersonId == id && 
                        p.MovieCasts.Any(mc => mc.MovieCastRoleTypes.Any(rt => rt.RoleType.RoleTypeName == "Director")));

                if (director == null)
                {
                    return NotFound();
                }

                return View(director);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in Details action for director ID {id}");
                return View("Error");
            }
        }
    }
} 