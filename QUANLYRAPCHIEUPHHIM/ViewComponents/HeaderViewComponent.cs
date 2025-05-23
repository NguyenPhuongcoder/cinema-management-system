using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.ViewModels;
using System.Linq;
using QUANLYRAPCHIEUPHHIM.Data;

namespace QUANLYRAPCHIEUPHHIM.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly CinemaDbcontext _context;

        public HeaderViewComponent(CinemaDbcontext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Lấy RoomFormat IMAX 1 lần, không lặp lại truy vấn
            var imaxFormat = _context.RoomFormats
                .AsNoTracking()
                .FirstOrDefault(f => f.FormatName == "IMAX");

            // Truy vấn MovieFormat chỉ khi cần
            var imaxMovieIds = imaxFormat != null
                ? _context.MovieFormats
                    .AsNoTracking()
                    .Where(mf => mf.FormatId == imaxFormat.FormatId)
                    .Select(mf => mf.MovieId)
                    .Distinct()
                : Enumerable.Empty<int>().AsQueryable();

            // Truy vấn phim, gom chung để tránh gọi nhiều lần db
            var nowShowingQuery = _context.Movies
                .AsNoTracking()
                .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value <= today);

            var comingSoonQuery = _context.Movies
                .AsNoTracking()
                .Where(m => m.ReleaseDate.HasValue && m.ReleaseDate.Value > today);

            var imaxMoviesQuery = _context.Movies
                .AsNoTracking()
                .Where(m => imaxMovieIds.Contains(m.MovieId));

            // Thực thi các truy vấn với Take và Select để lấy đúng dữ liệu cần thiết
            var nowShowing = nowShowingQuery
                .OrderByDescending(m => m.ReleaseDate)
                .Take(4)
                .Select(m => new MovieHeaderViewModel
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    PosterUrl = m.PosterUrl,
                    AgeLimit = m.AgeLimit,
                    Rating = m.Rating ?? 0
                })
                .ToList();

            var comingSoon = comingSoonQuery
                .OrderBy(m => m.ReleaseDate)
                .Take(4)
                .Select(m => new MovieHeaderViewModel
                {
                    MovieId = m.MovieId,
                    Title = m.Title,
                    PosterUrl = m.PosterUrl,
                    AgeLimit = m.AgeLimit,
                    Rating = m.Rating ?? 0
                })
                .ToList();

            var imaxMovies = imaxFormat != null
                ? imaxMoviesQuery
                    .Take(4)
                    .Select(m => new MovieHeaderViewModel
                    {
                        MovieId = m.MovieId,
                        Title = m.Title,
                        PosterUrl = m.PosterUrl,
                        AgeLimit = m.AgeLimit,
                        Rating = m.Rating ?? 0
                    })
                    .ToList()
                : new List<MovieHeaderViewModel>();

            // Lấy danh sách rạp
            var cinemas = _context.Cinemas
                .AsNoTracking()
                .Select(c => new CinemaHeaderViewModel
                {
                    Id = c.CinemaId,
                    Name = c.CinemaName
                })
                .ToList();

            var model = new HeaderViewModel
            {
                NowShowing = nowShowing,
                ComingSoon = comingSoon,
                ImaxMovies = imaxMovies,
                Cinemas = cinemas,
                UserName = User.Identity.IsAuthenticated ? User.Identity.Name : null,
                AvatarUrl = "/images/default-avatar.png"
            };

            return View("header", model);
        }

    }
}
