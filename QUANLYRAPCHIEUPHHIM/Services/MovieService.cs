using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Movie>> GetMoviesAsync(
            string? title = null,
            string? status = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Movies
                    .Include(m => m.MovieFormats)
                    .Include(m => m.Showtimes)
                    .AsQueryable();

                // Lọc theo tiêu đề nếu có
                if (!string.IsNullOrWhiteSpace(title))
                {
                    query = query.Where(m => m.Title.Contains(title));
                }

                // Phân trang
                return await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting movies");
                throw;
            }
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
        {
            try
            {
                return await _context.Movies
                    .Include(m => m.MovieFormats)
                    .Include(m => m.Showtimes)
                    .OrderByDescending(m => m.ReleaseDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all movies");
                throw;
            }
        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            try
            {
                return await _context.Movies
                    .Include(m => m.MovieFormats)
                    .Include(m => m.Showtimes)
                    .FirstOrDefaultAsync(m => m.MovieId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie by id {MovieId}", id);
                throw;
            }
        }


        public async Task<Movie> CreateMovieAsync(Movie movie)
        {
            try
            {
                // Validate movie data
                if (string.IsNullOrEmpty(movie.Title))
                    throw new ArgumentException("Movie title is required");

                if (movie.ReleaseDate == default)
                    throw new ArgumentException("Release date is required");

                // Check if movie title is unique
                var existingMovie = await _context.Movies
                    .FirstOrDefaultAsync(m => m.Title == movie.Title);

                if (existingMovie != null)
                    throw new ArgumentException("Movie title already exists");

                movie.CreatedAt = DateTime.Now;
                movie.UpdatedAt = DateTime.Now;

                _context.Movies.Add(movie);
                await _context.SaveChangesAsync();
                return movie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie");
                throw;
            }
        }

        public async Task<Movie> UpdateMovieAsync(Movie movie)
        {
            try
            {
                // Validate movie data
                if (string.IsNullOrEmpty(movie.Title))
                    throw new ArgumentException("Movie title is required");

                if (movie.ReleaseDate == default)
                    throw new ArgumentException("Release date is required");

                // Check if movie exists
                var existingMovie = await _context.Movies
                    .FirstOrDefaultAsync(m => m.MovieId == movie.MovieId);

                if (existingMovie == null)
                    throw new ArgumentException("Movie not found");

                // Check if movie title is unique
                var duplicateMovie = await _context.Movies
                    .FirstOrDefaultAsync(m => m.Title == movie.Title && 
                                            m.MovieId != movie.MovieId);

                if (duplicateMovie != null)
                    throw new ArgumentException("Movie title already exists");

                // Update movie properties
                existingMovie.Title = movie.Title;
                existingMovie.Description = movie.Description;
                existingMovie.Duration = movie.Duration;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.PosterUrl = movie.PosterUrl;
                existingMovie.TrailerUrl = movie.TrailerUrl;
                existingMovie.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingMovie;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie");
                throw;
            }
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.MovieFormats)
                    .Include(m => m.Showtimes)
                    .FirstOrDefaultAsync(m => m.MovieId == id);

                if (movie == null)
                    return false;

                // Check if movie has any showtimes
                if (movie.Showtimes.Any())
                    throw new InvalidOperationException("Cannot delete movie that has showtimes");

                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie");
                throw;
            }
        }

        public async Task<bool> MovieExistsAsync(int id)
        {
            try
            {
                return await _context.Movies.AnyAsync(m => m.MovieId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if movie exists");
                throw;
            }
        }
    }
} 