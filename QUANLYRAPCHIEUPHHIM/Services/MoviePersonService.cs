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
    public class MoviePersonService : IMoviePersonService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<MoviePersonService> _logger;

        public MoviePersonService(CinemaDbContext context, ILogger<MoviePersonService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MoviePerson>> GetMoviePersonsAsync(
            string? fullName = null,
            string? nationality = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.MoviePersons
                    .Include(p => p.MovieCasts)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(fullName))
                {
                    query = query.Where(p => p.FullName.Contains(fullName));
                }

                if (!string.IsNullOrEmpty(nationality))
                {
                    query = query.Where(p => p.Nationality.Contains(nationality));
                }

                return await query
                    .OrderBy(p => p.FullName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie persons");
                throw;
            }
        }

        public async Task<int> CountMoviePersonsAsync(
            string? fullName = null,
            string? nationality = null)
        {
            try
            {
                var query = _context.MoviePersons.AsQueryable();

                if (!string.IsNullOrEmpty(fullName))
                {
                    query = query.Where(p => p.FullName.Contains(fullName));
                }

                if (!string.IsNullOrEmpty(nationality))
                {
                    query = query.Where(p => p.Nationality.Contains(nationality));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting movie persons");
                throw;
            }
        }

        public async Task<MoviePerson> GetMoviePersonByIdAsync(int id)
        {
            try
            {
                return await _context.MoviePersons
                    .Include(p => p.MovieCasts)
                    .FirstOrDefaultAsync(p => p.PersonId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie person by id {PersonId}", id);
                throw;
            }
        }

        public async Task<MoviePerson> CreateMoviePersonAsync(MoviePerson person)
        {
            try
            {
                // Validate person data
                if (string.IsNullOrEmpty(person.FullName))
                    throw new ArgumentException("Full name is required");

                // Check if person name is unique
                var existingPerson = await _context.MoviePersons
                    .FirstOrDefaultAsync(p => p.FullName == person.FullName);

                if (existingPerson != null)
                    throw new ArgumentException("Person with this name already exists");

                person.CreatedAt = DateTime.Now;
                person.UpdatedAt = DateTime.Now;

                _context.MoviePersons.Add(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie person");
                throw;
            }
        }

        public async Task<MoviePerson> UpdateMoviePersonAsync(MoviePerson person)
        {
            try
            {
                // Validate person data
                if (string.IsNullOrEmpty(person.FullName))
                    throw new ArgumentException("Full name is required");

                // Check if person exists
                var existingPerson = await _context.MoviePersons
                    .FirstOrDefaultAsync(p => p.PersonId == person.PersonId);

                if (existingPerson == null)
                    throw new ArgumentException("Person not found");

                // Check if person name is unique
                var duplicatePerson = await _context.MoviePersons
                    .FirstOrDefaultAsync(p => p.FullName == person.FullName && 
                                            p.PersonId != person.PersonId);

                if (duplicatePerson != null)
                    throw new ArgumentException("Person with this name already exists");

                existingPerson.FullName = person.FullName;
                existingPerson.BirthDate = person.BirthDate;
                existingPerson.Nationality = person.Nationality;
                existingPerson.Biography = person.Biography;
                existingPerson.ImageUrl = person.ImageUrl;
                existingPerson.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingPerson;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie person");
                throw;
            }
        }

        public async Task<bool> DeleteMoviePersonAsync(int id)
        {
            try
            {
                var person = await _context.MoviePersons
                    .Include(p => p.MovieCasts)
                    .FirstOrDefaultAsync(p => p.PersonId == id);

                if (person == null)
                    return false;

                // Check if person is used by any movie
                if (person.MovieCasts.Any())
                    throw new InvalidOperationException("Cannot delete person that is used by movies");

                _context.MoviePersons.Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie person");
                throw;
            }
        }
    }
} 