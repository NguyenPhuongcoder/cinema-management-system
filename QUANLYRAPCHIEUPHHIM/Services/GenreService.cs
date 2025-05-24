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
    public class GenreService : IGenreService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<GenreService> _logger;

        public GenreService(CinemaDbContext context, ILogger<GenreService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Genre>> GetGenresAsync(
            string? genreName = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Genres
                    .Include(g => g.MovieGenres)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(genreName))
                {
                    query = query.Where(g => g.GenreName.Contains(genreName));
                }

                return await query
                    .OrderBy(g => g.GenreName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genres");
                throw;
            }
        }

        public async Task<int> CountGenresAsync(string? genreName = null)
        {
            try
            {
                var query = _context.Genres.AsQueryable();

                if (!string.IsNullOrEmpty(genreName))
                {
                    query = query.Where(g => g.GenreName.Contains(genreName));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting genres");
                throw;
            }
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            try
            {
                return await _context.Genres
                    .Include(g => g.MovieGenres)
                    .FirstOrDefaultAsync(g => g.GenreId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genre by id {GenreId}", id);
                throw;
            }
        }

        public async Task<Genre> CreateGenreAsync(Genre genre)
        {
            try
            {
                // Validate genre data
                if (string.IsNullOrEmpty(genre.GenreName))
                    throw new ArgumentException("Genre name is required");

                // Check if genre name is unique
                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.GenreName == genre.GenreName);

                if (existingGenre != null)
                    throw new ArgumentException("Genre name already exists");

                genre.CreatedAt = DateTime.Now;
                genre.UpdatedAt = DateTime.Now;

                _context.Genres.Add(genre);
                await _context.SaveChangesAsync();
                return genre;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating genre");
                throw;
            }
        }

        public async Task<Genre> UpdateGenreAsync(Genre genre)
        {
            try
            {
                // Validate genre data
                if (string.IsNullOrEmpty(genre.GenreName))
                    throw new ArgumentException("Genre name is required");

                // Check if genre exists
                var existingGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.GenreId == genre.GenreId);

                if (existingGenre == null)
                    throw new ArgumentException("Genre not found");

                // Check if genre name is unique
                var duplicateGenre = await _context.Genres
                    .FirstOrDefaultAsync(g => g.GenreName == genre.GenreName && 
                                            g.GenreId != genre.GenreId);

                if (duplicateGenre != null)
                    throw new ArgumentException("Genre name already exists");

                existingGenre.GenreName = genre.GenreName;
                existingGenre.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingGenre;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating genre");
                throw;
            }
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            try
            {
                var genre = await _context.Genres
                    .Include(g => g.MovieGenres)
                    .FirstOrDefaultAsync(g => g.GenreId == id);

                if (genre == null)
                    return false;

                // Check if genre is used by any movie
                if (genre.MovieGenres.Any())
                    throw new InvalidOperationException("Cannot delete genre that is used by movies");

                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting genre");
                throw;
            }
        }
    }
} 