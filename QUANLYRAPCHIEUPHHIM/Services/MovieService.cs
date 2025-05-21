using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public class MovieService : IMovieService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<MovieService> _logger;

        public MovieService(CinemaDbContext context, ILogger<MovieService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            return await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieFormats)
                    .ThenInclude(mf => mf.RoomFormat)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieFormats)
                    .ThenInclude(mf => mf.RoomFormat)
                .FirstOrDefaultAsync(m => m.MovieId == id);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieFormats)
                    .ThenInclude(mf => mf.RoomFormat)
                .Where(m => m.ReleaseDate >= startDate && m.ReleaseDate <= endDate)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> GetActiveMoviesAsync()
        {
            var currentDate = DateTime.Now;
            return await _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieFormats)
                    .ThenInclude(mf => mf.RoomFormat)
                .Where(m => m.ReleaseDate <= currentDate)
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Movie>> SearchMoviesAsync(
            string? title = null,
            string? genre = null,
            string? format = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool? isActive = null)
        {
            var query = _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieFormats)
                    .ThenInclude(mf => mf.RoomFormat)
                .AsQueryable();

            if (!string.IsNullOrEmpty(title))
                query = query.Where(m => m.Title.Contains(title));

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(m => m.MovieGenres.Any(mg => mg.Genre.GenreName.Contains(genre)));

            if (!string.IsNullOrEmpty(format))
                query = query.Where(m => m.MovieFormats.Any(mf => mf.RoomFormat.FormatName.Contains(format)));

            if (startDate.HasValue)
                query = query.Where(m => m.ReleaseDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.ReleaseDate <= endDate.Value);

            if (isActive.HasValue)
            {
                var currentDate = DateTime.Now;
                if (isActive.Value)
                    query = query.Where(m => m.ReleaseDate <= currentDate);
                else
                    query = query.Where(m => m.ReleaseDate > currentDate);
            }

            return await query
                .OrderByDescending(m => m.ReleaseDate)
                .ToListAsync();
        }

        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            movie.CreatedAt = DateTime.Now;
            movie.UpdatedAt = DateTime.Now;
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie> UpdateMovieAsync(Movie movie)
        {
            movie.UpdatedAt = DateTime.Now;
            _context.Entry(movie).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
                return false;

            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 