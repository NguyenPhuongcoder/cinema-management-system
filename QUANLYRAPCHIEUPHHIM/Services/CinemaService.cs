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
    public class CinemaService : ICinemaService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<CinemaService> _logger;

        public CinemaService(CinemaDbContext context, ILogger<CinemaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Cinema>> GetCinemasAsync()
        {
            try
            {
                return await _context.Cinemas
                    .Include(c => c.Rooms)
                    .Include(c => c.Address)
                    .OrderBy(c => c.CinemaName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all cinemas");
                throw;
            }
        }

        public async Task<IEnumerable<Cinema>> GetCinemasAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.Cinemas
                    .Include(c => c.Rooms)
                    .Include(c => c.Address)
                    .OrderBy(c => c.CinemaName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting cinemas with pagination");
                throw;
            }
        }

        public async Task<int> CountCinemasAsync()
        {
            try
            {
                return await _context.Cinemas.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while counting cinemas");
                throw;
            }
        }

        public async Task<Cinema> GetCinemaByIdAsync(int id)
        {
            try
            {
                var cinema = await _context.Cinemas
                    .Include(c => c.Rooms)
                    .Include(c => c.Address)
                    .FirstOrDefaultAsync(c => c.CinemaId == id);

                if (cinema == null)
                {
                    throw new KeyNotFoundException($"Cinema with ID {id} not found");
                }

                return cinema;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting cinema with ID {id}");
                throw;
            }
        }

        public async Task<Cinema> CreateCinemaAsync(Cinema cinema)
        {
            try
            {
                // Kiểm tra trùng lặp tên rạp
                if (await _context.Cinemas.AnyAsync(c => c.CinemaName == cinema.CinemaName))
                {
                    throw new InvalidOperationException($"Cinema with name '{cinema.CinemaName}' already exists");
                }

                _context.Cinemas.Add(cinema);
                await _context.SaveChangesAsync();
                return cinema;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating cinema");
                throw;
            }
        }

        public async Task<Cinema> UpdateCinemaAsync(Cinema cinema)
        {
            try
            {
                var existingCinema = await _context.Cinemas.FindAsync(cinema.CinemaId);
                if (existingCinema == null)
                {
                    throw new KeyNotFoundException($"Cinema with ID {cinema.CinemaId} not found");
                }

                // Kiểm tra trùng lặp tên rạp (nếu tên đã thay đổi)
                if (cinema.CinemaName != existingCinema.CinemaName &&
                    await _context.Cinemas.AnyAsync(c => c.CinemaName == cinema.CinemaName))
                {
                    throw new InvalidOperationException($"Cinema with name '{cinema.CinemaName}' already exists");
                }

                _context.Entry(existingCinema).CurrentValues.SetValues(cinema);
                await _context.SaveChangesAsync();
                return cinema;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating cinema with ID {cinema.CinemaId}");
                throw;
            }
        }

        public async Task DeleteCinemaAsync(int id)
        {
            try
            {
                var cinema = await _context.Cinemas
                    .Include(c => c.Rooms)
                    .FirstOrDefaultAsync(c => c.CinemaId == id);

                if (cinema == null)
                {
                    throw new KeyNotFoundException($"Cinema with ID {id} not found");
                }

                // Kiểm tra xem rạp có phòng chiếu nào không
                if (cinema.Rooms.Any())
                {
                    throw new InvalidOperationException("Cannot delete cinema with existing rooms");
                }

                _context.Cinemas.Remove(cinema);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting cinema with ID {id}");
                throw;
            }
        }

        public async Task<bool> CinemaExistsAsync(int id)
        {
            return await _context.Cinemas.AnyAsync(c => c.CinemaId == id);
        }

        public async Task<IEnumerable<Cinema>> GetCinemasByCityAsync(int cityId)
        {
            try
            {
                return await _context.Cinemas
                    .Include(c => c.Address)
                    .Include(c => c.Rooms)
                    .Where(c => c.AddressId == cityId)
                    .OrderBy(c => c.CinemaName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting cinemas by city ID {cityId}");
                throw;
            }
        }
    }
} 