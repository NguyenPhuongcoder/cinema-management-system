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
    public class CityService : ICityService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<CityService> _logger;

        public CityService(CinemaDbContext context, ILogger<CityService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(
            string? cityName = null,
            int? provinceId = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Cities
                    .Include(c => c.Province)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(cityName))
                {
                    query = query.Where(c => c.CityName.Contains(cityName));
                }

                if (provinceId.HasValue)
                {
                    query = query.Where(c => c.ProvinceId == provinceId.Value);
                }

                return await query
                    .OrderBy(c => c.Province.ProvinceName)
                    .ThenBy(c => c.CityName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cities");
                throw;
            }
        }

        public async Task<int> CountCitiesAsync(
            string? cityName = null,
            int? provinceId = null)
        {
            try
            {
                var query = _context.Cities.AsQueryable();

                if (!string.IsNullOrEmpty(cityName))
                {
                    query = query.Where(c => c.CityName.Contains(cityName));
                }

                if (provinceId.HasValue)
                {
                    query = query.Where(c => c.ProvinceId == provinceId.Value);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting cities");
                throw;
            }
        }

        public async Task<City> GetCityByIdAsync(int id)
        {
            try
            {
                return await _context.Cities
                    .Include(c => c.Province)
                    .FirstOrDefaultAsync(c => c.CityId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting city by id {CityId}", id);
                throw;
            }
        }

        public async Task<City> CreateCityAsync(City city)
        {
            try
            {
                // Validate city data
                if (string.IsNullOrEmpty(city.CityName))
                    throw new ArgumentException("City name is required");

                if (city.ProvinceId <= 0)
                    throw new ArgumentException("Province ID is required");

                // Check if province exists
                var province = await _context.Provinces
                    .FirstOrDefaultAsync(p => p.ProvinceId == city.ProvinceId);

                if (province == null)
                    throw new ArgumentException("Province not found");

                // Check if city name is unique in the province
                var existingCity = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityName == city.CityName && c.ProvinceId == city.ProvinceId);

                if (existingCity != null)
                    throw new ArgumentException("City name already exists in this province");

                _context.Cities.Add(city);
                await _context.SaveChangesAsync();
                return city;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating city");
                throw;
            }
        }

        public async Task<City> UpdateCityAsync(City city)
        {
            try
            {
                // Validate city data
                if (string.IsNullOrEmpty(city.CityName))
                    throw new ArgumentException("City name is required");

                if (city.ProvinceId <= 0)
                    throw new ArgumentException("Province ID is required");

                // Check if city exists
                var existingCity = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityId == city.CityId);

                if (existingCity == null)
                    throw new ArgumentException("City not found");

                // Check if province exists
                var province = await _context.Provinces
                    .FirstOrDefaultAsync(p => p.ProvinceId == city.ProvinceId);

                if (province == null)
                    throw new ArgumentException("Province not found");

                // Check if city name is unique in the province
                var duplicateCity = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityName == city.CityName && 
                                            c.ProvinceId == city.ProvinceId && 
                                            c.CityId != city.CityId);

                if (duplicateCity != null)
                    throw new ArgumentException("City name already exists in this province");

                existingCity.CityName = city.CityName;
                existingCity.ProvinceId = city.ProvinceId;

                await _context.SaveChangesAsync();
                return existingCity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating city");
                throw;
            }
        }

        public async Task<bool> DeleteCityAsync(int id)
        {
            try
            {
                var city = await _context.Cities
                    .Include(c => c.Addresses)
                    .FirstOrDefaultAsync(c => c.CityId == id);

                if (city == null)
                    return false;

                // Check if city is used by any address
                if (city.Addresses.Any())
                    throw new InvalidOperationException("Cannot delete city that is used by addresses");

                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting city");
                throw;
            }
        }

        public async Task<IEnumerable<City>> GetCitiesByProvinceAsync(int provinceId)
        {
            try
            {
                return await _context.Cities
                    .Include(c => c.Province)
                    .Where(c => c.ProvinceId == provinceId)
                    .OrderBy(c => c.CityName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cities by province {ProvinceId}", provinceId);
                throw;
            }
        }
    }
} 