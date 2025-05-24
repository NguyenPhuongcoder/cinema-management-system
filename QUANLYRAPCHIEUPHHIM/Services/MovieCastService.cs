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
    public class MovieCastService : IMovieCastService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<MovieCastService> _logger;

        public MovieCastService(CinemaDbContext context, ILogger<MovieCastService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MovieCast>> GetMovieCastsAsync(
            int? movieId = null,
            int? personId = null,
            string? characterName = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.MovieCasts
                    .Include(mc => mc.Movie)
                    .Include(mc => mc.Person)
                    .Include(mc => mc.MovieCastRoleTypes)
                        .ThenInclude(mcrt => mcrt.RoleType)
                    .AsQueryable();

                if (movieId.HasValue)
                {
                    query = query.Where(mc => mc.MovieId == movieId.Value);
                }

                if (personId.HasValue)
                {
                    query = query.Where(mc => mc.PersonId == personId.Value);
                }

                if (!string.IsNullOrEmpty(characterName))
                {
                    query = query.Where(mc => mc.CharacterName.Contains(characterName));
                }

                return await query
                    .OrderBy(mc => mc.Movie.Title)
                    .ThenBy(mc => mc.Person.FullName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie casts");
                throw;
            }
        }

        public async Task<int> CountMovieCastsAsync(
            int? movieId = null,
            int? personId = null,
            string? characterName = null)
        {
            try
            {
                var query = _context.MovieCasts.AsQueryable();

                if (movieId.HasValue)
                {
                    query = query.Where(mc => mc.MovieId == movieId.Value);
                }

                if (personId.HasValue)
                {
                    query = query.Where(mc => mc.PersonId == personId.Value);
                }

                if (!string.IsNullOrEmpty(characterName))
                {
                    query = query.Where(mc => mc.CharacterName.Contains(characterName));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting movie casts");
                throw;
            }
        }

        public async Task<MovieCast> GetMovieCastByIdAsync(int id)
        {
            try
            {
                return await _context.MovieCasts
                    .Include(mc => mc.Movie)
                    .Include(mc => mc.Person)
                    .Include(mc => mc.MovieCastRoleTypes)
                        .ThenInclude(mcrt => mcrt.RoleType)
                    .FirstOrDefaultAsync(mc => mc.MovieCastId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie cast by id {MovieCastId}", id);
                throw;
            }
        }

        public async Task<MovieCast> CreateMovieCastAsync(MovieCast movieCast)
        {
            try
            {
                // Validate movie cast data
                if (movieCast.MovieId <= 0)
                    throw new ArgumentException("Movie ID is required");

                if (movieCast.PersonId <= 0)
                    throw new ArgumentException("Person ID is required");

                // Check if movie exists
                var movie = await _context.Movies.FindAsync(movieCast.MovieId);
                if (movie == null)
                    throw new ArgumentException("Movie not found");

                // Check if person exists
                var person = await _context.MoviePersons.FindAsync(movieCast.PersonId);
                if (person == null)
                    throw new ArgumentException("Person not found");

                // Check if movie cast already exists
                var existingCast = await _context.MovieCasts
                    .FirstOrDefaultAsync(mc => mc.MovieId == movieCast.MovieId && 
                                             mc.PersonId == movieCast.PersonId);

                if (existingCast != null)
                    throw new ArgumentException("This person is already cast in this movie");

                _context.MovieCasts.Add(movieCast);
                await _context.SaveChangesAsync();
                return movieCast;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie cast");
                throw;
            }
        }

        public async Task<MovieCast> UpdateMovieCastAsync(MovieCast movieCast)
        {
            try
            {
                // Validate movie cast data
                if (movieCast.MovieId <= 0)
                    throw new ArgumentException("Movie ID is required");

                if (movieCast.PersonId <= 0)
                    throw new ArgumentException("Person ID is required");

                // Check if movie cast exists
                var existingCast = await _context.MovieCasts
                    .Include(mc => mc.MovieCastRoleTypes)
                    .FirstOrDefaultAsync(mc => mc.MovieCastId == movieCast.MovieCastId);

                if (existingCast == null)
                    throw new ArgumentException("Movie cast not found");

                // Check if movie exists
                var movie = await _context.Movies.FindAsync(movieCast.MovieId);
                if (movie == null)
                    throw new ArgumentException("Movie not found");

                // Check if person exists
                var person = await _context.MoviePersons.FindAsync(movieCast.PersonId);
                if (person == null)
                    throw new ArgumentException("Person not found");

                // Check if movie cast already exists (excluding current cast)
                var duplicateCast = await _context.MovieCasts
                    .FirstOrDefaultAsync(mc => mc.MovieId == movieCast.MovieId && 
                                             mc.PersonId == movieCast.PersonId &&
                                             mc.MovieCastId != movieCast.MovieCastId);

                if (duplicateCast != null)
                    throw new ArgumentException("This person is already cast in this movie");

                // Update movie cast
                existingCast.MovieId = movieCast.MovieId;
                existingCast.PersonId = movieCast.PersonId;
                existingCast.CharacterName = movieCast.CharacterName;

                // Update role types
                _context.MovieCastRoleTypes.RemoveRange(existingCast.MovieCastRoleTypes);
                foreach (var roleType in movieCast.MovieCastRoleTypes)
                {
                    roleType.MovieCastId = existingCast.MovieCastId;
                    _context.MovieCastRoleTypes.Add(roleType);
                }

                await _context.SaveChangesAsync();
                return existingCast;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie cast");
                throw;
            }
        }

        public async Task<bool> DeleteMovieCastAsync(int id)
        {
            try
            {
                var movieCast = await _context.MovieCasts
                    .Include(mc => mc.MovieCastRoleTypes)
                    .FirstOrDefaultAsync(mc => mc.MovieCastId == id);

                if (movieCast == null)
                    return false;

                _context.MovieCastRoleTypes.RemoveRange(movieCast.MovieCastRoleTypes);
                _context.MovieCasts.Remove(movieCast);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie cast");
                throw;
            }
        }

        public async Task<IEnumerable<MovieCast>> GetMovieCastsByMovieAsync(int movieId)
        {
            try
            {
                return await _context.MovieCasts
                    .Include(mc => mc.Movie)
                    .Include(mc => mc.Person)
                    .Include(mc => mc.MovieCastRoleTypes)
                        .ThenInclude(mcrt => mcrt.RoleType)
                    .Where(mc => mc.MovieId == movieId)
                    .OrderBy(mc => mc.Person.FullName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie casts by movie {MovieId}", movieId);
                throw;
            }
        }

        public async Task<IEnumerable<MovieCast>> GetMovieCastsByPersonAsync(int personId)
        {
            try
            {
                return await _context.MovieCasts
                    .Include(mc => mc.Movie)
                    .Include(mc => mc.Person)
                    .Include(mc => mc.MovieCastRoleTypes)
                        .ThenInclude(mcrt => mcrt.RoleType)
                    .Where(mc => mc.PersonId == personId)
                    .OrderBy(mc => mc.Movie.Title)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie casts by person {PersonId}", personId);
                throw;
            }
        }
    }
} 