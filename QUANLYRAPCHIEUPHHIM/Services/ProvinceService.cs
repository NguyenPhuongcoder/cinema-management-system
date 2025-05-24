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
    public class ProvinceService : IProvinceService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<ProvinceService> _logger;

        public ProvinceService(CinemaDbContext context, ILogger<ProvinceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Province>> GetProvincesAsync(
            string? provinceName = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Provinces
                    .Include(p => p.Cities)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(provinceName))
                {
                    query = query.Where(p => p.ProvinceName.Contains(provinceName));
                }

                return await query
                    .OrderBy(p => p.ProvinceName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting provinces");
                throw;
            }
        }

        public async Task<int> CountProvincesAsync(string? provinceName = null)
        {
            try
            {
                var query = _context.Provinces.AsQueryable();

                if (!string.IsNullOrEmpty(provinceName))
                {
                    query = query.Where(p => p.ProvinceName.Contains(provinceName));
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting provinces");
                throw;
            }
        }

        public async Task<Province> GetProvinceByIdAsync(int id)
        {
            try
            {
                return await _context.Provinces
                    .Include(p => p.Cities)
                    .FirstOrDefaultAsync(p => p.ProvinceId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting province by id {ProvinceId}", id);
                throw;
            }
        }

        public async Task<Province> CreateProvinceAsync(Province province)
        {
            try
            {
                // Validate province data
                if (string.IsNullOrEmpty(province.ProvinceName))
                    throw new ArgumentException("Province name is required");

                // Check if province name is unique
                var existingProvince = await _context.Provinces
                    .FirstOrDefaultAsync(p => p.ProvinceName == province.ProvinceName);

                if (existingProvince != null)
                    throw new ArgumentException("Province name already exists");

                _context.Provinces.Add(province);
                await _context.SaveChangesAsync();
                return province;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating province");
                throw;
            }
        }

        public async Task<Province> UpdateProvinceAsync(Province province)
        {
            try
            {
                // Validate province data
                if (string.IsNullOrEmpty(province.ProvinceName))
                    throw new ArgumentException("Province name is required");

                // Check if province exists
                var existingProvince = await _context.Provinces
                    .FirstOrDefaultAsync(p => p.ProvinceId == province.ProvinceId);

                if (existingProvince == null)
                    throw new ArgumentException("Province not found");

                // Check if province name is unique
                var duplicateProvince = await _context.Provinces
                    .FirstOrDefaultAsync(p => p.ProvinceName == province.ProvinceName && 
                                            p.ProvinceId != province.ProvinceId);

                if (duplicateProvince != null)
                    throw new ArgumentException("Province name already exists");

                existingProvince.ProvinceName = province.ProvinceName;

                await _context.SaveChangesAsync();
                return existingProvince;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating province");
                throw;
            }
        }

        public async Task<bool> DeleteProvinceAsync(int id)
        {
            try
            {
                var province = await _context.Provinces
                    .Include(p => p.Cities)
                    .FirstOrDefaultAsync(p => p.ProvinceId == id);

                if (province == null)
                    return false;

                // Check if province is used by any city
                if (province.Cities.Any())
                    throw new InvalidOperationException("Cannot delete province that is used by cities");

                _context.Provinces.Remove(province);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting province");
                throw;
            }
        }
    }
} 