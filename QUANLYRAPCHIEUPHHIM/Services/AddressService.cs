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
    public class AddressService : IAddressService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<AddressService> _logger;

        public AddressService(CinemaDbContext context, ILogger<AddressService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Address>> GetAddressesAsync(
            string? addressDetail = null,
            int? cityId = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Addresses
                    .Include(a => a.City)
                        .ThenInclude(c => c.Province)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(addressDetail))
                {
                    query = query.Where(a => a.AddressDetail.Contains(addressDetail));
                }

                if (cityId.HasValue)
                {
                    query = query.Where(a => a.CityId == cityId.Value);
                }

                return await query
                    .OrderBy(a => a.City.CityName)
                    .ThenBy(a => a.AddressDetail)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses");
                throw;
            }
        }

        public async Task<int> CountAddressesAsync(
            string? addressDetail = null,
            int? cityId = null)
        {
            try
            {
                var query = _context.Addresses.AsQueryable();

                if (!string.IsNullOrEmpty(addressDetail))
                {
                    query = query.Where(a => a.AddressDetail.Contains(addressDetail));
                }

                if (cityId.HasValue)
                {
                    query = query.Where(a => a.CityId == cityId.Value);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting addresses");
                throw;
            }
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            try
            {
                return await _context.Addresses
                    .Include(a => a.City)
                        .ThenInclude(c => c.Province)
                    .FirstOrDefaultAsync(a => a.AddressId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address by id {AddressId}", id);
                throw;
            }
        }

        public async Task<Address> CreateAddressAsync(Address address)
        {
            try
            {
                // Validate address data
                if (string.IsNullOrEmpty(address.AddressDetail))
                    throw new ArgumentException("Address detail is required");

                if (address.CityId <= 0)
                    throw new ArgumentException("City ID is required");

                // Check if city exists
                var city = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityId == address.CityId);

                if (city == null)
                    throw new ArgumentException("City not found");

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                return address;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating address");
                throw;
            }
        }

        public async Task<Address> UpdateAddressAsync(Address address)
        {
            try
            {
                // Validate address data
                if (string.IsNullOrEmpty(address.AddressDetail))
                    throw new ArgumentException("Address detail is required");

                if (address.CityId <= 0)
                    throw new ArgumentException("City ID is required");

                // Check if address exists
                var existingAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressId == address.AddressId);

                if (existingAddress == null)
                    throw new ArgumentException("Address not found");

                // Check if city exists
                var city = await _context.Cities
                    .FirstOrDefaultAsync(c => c.CityId == address.CityId);

                if (city == null)
                    throw new ArgumentException("City not found");

                existingAddress.AddressDetail = address.AddressDetail;
                existingAddress.CityId = address.CityId;

                await _context.SaveChangesAsync();
                return existingAddress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address");
                throw;
            }
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            try
            {
                var address = await _context.Addresses
                    .Include(a => a.Cinemas)
                    .FirstOrDefaultAsync(a => a.AddressId == id);

                if (address == null)
                    return false;

                // Check if address is used by any cinema
                if (address.Cinemas.Any())
                    throw new InvalidOperationException("Cannot delete address that is used by cinemas");

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address");
                throw;
            }
        }

        public async Task<IEnumerable<Address>> GetAddressesByCityAsync(int cityId)
        {
            try
            {
                return await _context.Addresses
                    .Include(a => a.City)
                        .ThenInclude(c => c.Province)
                    .Where(a => a.CityId == cityId)
                    .OrderBy(a => a.AddressDetail)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting addresses by city {CityId}", cityId);
                throw;
            }
        }
    }
} 