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
    public class FormatService : IFormatService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<FormatService> _logger;

        public FormatService(CinemaDbContext context, ILogger<FormatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RoomFormat>> GetFormatsAsync(
            string? formatName = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.RoomFormats
                    .Include(f => f.Rooms)
                    .Include(f => f.MovieFormats)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(formatName))
                {
                    query = query.Where(f => f.FormatName.Contains(formatName));
                }

                return await query
                    .OrderBy(f => f.FormatName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting formats");
                throw;
            }
        }

        public async Task<int> CountFormatsAsync(string? formatName = null)
        {
            try
            {
                var query = _context.RoomFormats.AsQueryable();

                if (!string.IsNullOrEmpty(formatName))
                {
                    query = query.Where(f => f.FormatName.Contains(formatName));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting formats");
                throw;
            }
        }

        public async Task<RoomFormat> GetFormatByIdAsync(int id)
        {
            try
            {
                return await _context.RoomFormats
                    .Include(f => f.Rooms)
                    .Include(f => f.MovieFormats)
                    .FirstOrDefaultAsync(f => f.FormatId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting format by id {FormatId}", id);
                throw;
            }
        }

        public async Task<RoomFormat> CreateFormatAsync(RoomFormat format)
        {
            try
            {
                // Validate format data
                if (string.IsNullOrEmpty(format.FormatName))
                    throw new ArgumentException("Format name is required");

                // Check if format name is unique
                var existingFormat = await _context.RoomFormats
                    .FirstOrDefaultAsync(f => f.FormatName == format.FormatName);

                if (existingFormat != null)
                    throw new ArgumentException("Format name already exists");

                format.CreatedAt = DateTime.Now;
                format.UpdatedAt = DateTime.Now;

                _context.RoomFormats.Add(format);
                await _context.SaveChangesAsync();
                return format;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating format");
                throw;
            }
        }

        public async Task<RoomFormat> UpdateFormatAsync(RoomFormat format)
        {
            try
            {
                // Validate format data
                if (string.IsNullOrEmpty(format.FormatName))
                    throw new ArgumentException("Format name is required");

                // Check if format exists
                var existingFormat = await _context.RoomFormats
                    .FirstOrDefaultAsync(f => f.FormatId == format.FormatId);

                if (existingFormat == null)
                    throw new ArgumentException("Format not found");

                // Check if format name is unique
                var duplicateFormat = await _context.RoomFormats
                    .FirstOrDefaultAsync(f => f.FormatName == format.FormatName && 
                                            f.FormatId != format.FormatId);

                if (duplicateFormat != null)
                    throw new ArgumentException("Format name already exists");

                existingFormat.FormatName = format.FormatName;
                existingFormat.Description = format.Description;
                existingFormat.AdditionalCharge = format.AdditionalCharge;
                existingFormat.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingFormat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating format");
                throw;
            }
        }

        public async Task<bool> DeleteFormatAsync(int id)
        {
            try
            {
                var format = await _context.RoomFormats
                    .Include(f => f.Rooms)
                    .Include(f => f.MovieFormats)
                    .FirstOrDefaultAsync(f => f.FormatId == id);

                if (format == null)
                    return false;

                // Check if format is used by any room
                if (format.Rooms.Any())
                    throw new InvalidOperationException("Cannot delete format that is used by rooms");

                // Check if format is used by any movie
                if (format.MovieFormats.Any())
                    throw new InvalidOperationException("Cannot delete format that is used by movies");

                _context.RoomFormats.Remove(format);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting format");
                throw;
            }
        }
    }
} 