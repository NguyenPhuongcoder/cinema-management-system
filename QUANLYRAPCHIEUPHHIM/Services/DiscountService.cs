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
    public class DiscountService : IDiscountService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(CinemaDbContext context, ILogger<DiscountService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Discount>> GetDiscountsAsync(
            string discountName = null,
            string couponCode = null,
            string discountType = null,
            bool? isActive = null,
            DateOnly? fromDate = null,
            DateOnly? toDate = null,
            decimal? minValue = null,
            decimal? maxValue = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Discounts
                    .Include(d => d.DiscountDiscountTypes)
                        .ThenInclude(ddt => ddt.DiscountType)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(discountName))
                {
                    query = query.Where(d => d.DiscountName.Contains(discountName));
                }

                if (!string.IsNullOrEmpty(couponCode))
                {
                    query = query.Where(d => d.CouponCode.Contains(couponCode));
                }

                if (!string.IsNullOrEmpty(discountType))
                {
                    query = query.Where(d => d.DiscountDiscountTypes.Any(ddt => ddt.DiscountType.DiscountTypeName == discountType));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(d => d.IsActive == isActive.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(d => d.StartDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(d => d.EndDate <= toDate.Value);
                }

                if (minValue.HasValue)
                {
                    query = query.Where(d => d.DiscountValue >= minValue.Value);
                }

                if (maxValue.HasValue)
                {
                    query = query.Where(d => d.DiscountValue <= maxValue.Value);
                }

                return await query
                    .OrderByDescending(d => d.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting discounts");
                throw;
            }
        }

        public async Task<int> CountDiscountsAsync(
            string discountName = null,
            string couponCode = null,
            string discountType = null,
            bool? isActive = null,
            DateOnly? fromDate = null,
            DateOnly? toDate = null,
            decimal? minValue = null,
            decimal? maxValue = null)
        {
            try
            {
                var query = _context.Discounts.AsQueryable();

                if (!string.IsNullOrEmpty(discountName))
                {
                    query = query.Where(d => d.DiscountName.Contains(discountName));
                }

                if (!string.IsNullOrEmpty(couponCode))
                {
                    query = query.Where(d => d.CouponCode.Contains(couponCode));
                }

                if (!string.IsNullOrEmpty(discountType))
                {
                    query = query.Where(d => d.DiscountDiscountTypes.Any(ddt => ddt.DiscountType.DiscountTypeName == discountType));
                }

                if (isActive.HasValue)
                {
                    query = query.Where(d => d.IsActive == isActive.Value);
                }

                if (fromDate.HasValue)
                {
                    query = query.Where(d => d.StartDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    query = query.Where(d => d.EndDate <= toDate.Value);
                }

                if (minValue.HasValue)
                {
                    query = query.Where(d => d.DiscountValue >= minValue.Value);
                }

                if (maxValue.HasValue)
                {
                    query = query.Where(d => d.DiscountValue <= maxValue.Value);
                }

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting discounts");
                throw;
            }
        }

        public async Task<Discount> GetDiscountByIdAsync(int id)
        {
            try
            {
                return await _context.Discounts
                    .Include(d => d.DiscountDiscountTypes)
                        .ThenInclude(ddt => ddt.DiscountType)
                    .FirstOrDefaultAsync(d => d.DiscountId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting discount by id {DiscountId}", id);
                throw;
            }
        }

        public async Task<Discount> GetDiscountByCouponCodeAsync(string couponCode)
        {
            return await _context.Discounts
                .Include(d => d.DiscountDiscountTypes)
                    .ThenInclude(ddt => ddt.DiscountType)
                .FirstOrDefaultAsync(d => d.CouponCode == couponCode);
        }

        public async Task<Discount> CreateDiscountAsync(Discount discount)
        {
            try
            {
                // Validate discount data
                if (string.IsNullOrEmpty(discount.DiscountName))
                    throw new ArgumentException("Discount name is required");

                if (string.IsNullOrEmpty(discount.CouponCode))
                    throw new ArgumentException("Coupon code is required");

                if (discount.StartDate >= discount.EndDate)
                    throw new ArgumentException("End date must be after start date");

                if (discount.DiscountValue <= 0)
                    throw new ArgumentException("Discount value must be greater than 0");

                // Check if coupon code is unique
                var existingDiscount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.CouponCode == discount.CouponCode);

                if (existingDiscount != null)
                    throw new ArgumentException("Coupon code already exists");

                discount.CreatedAt = DateTime.Now;
                discount.UpdatedAt = DateTime.Now;

                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
                return discount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating discount");
                throw;
            }
        }

        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            try
            {
                var existingDiscount = await _context.Discounts
                    .Include(d => d.DiscountDiscountTypes)
                    .FirstOrDefaultAsync(d => d.DiscountId == discount.DiscountId);

                if (existingDiscount == null)
                    throw new ArgumentException("Discount not found");

                // Validate discount data
                if (string.IsNullOrEmpty(discount.DiscountName))
                    throw new ArgumentException("Discount name is required");

                if (string.IsNullOrEmpty(discount.CouponCode))
                    throw new ArgumentException("Coupon code is required");

                if (discount.StartDate >= discount.EndDate)
                    throw new ArgumentException("End date must be after start date");

                if (discount.DiscountValue <= 0)
                    throw new ArgumentException("Discount value must be greater than 0");

                // Check if coupon code is unique (excluding current discount)
                var duplicateDiscount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.CouponCode == discount.CouponCode && d.DiscountId != discount.DiscountId);

                if (duplicateDiscount != null)
                    throw new ArgumentException("Coupon code already exists");

                // Update basic properties
                existingDiscount.DiscountName = discount.DiscountName;
                existingDiscount.CouponCode = discount.CouponCode;
                existingDiscount.DiscountValue = discount.DiscountValue;
                existingDiscount.StartDate = discount.StartDate;
                existingDiscount.EndDate = discount.EndDate;
                existingDiscount.IsActive = discount.IsActive;
                existingDiscount.UpdatedAt = DateTime.Now;

                // Update DiscountDiscountTypes
                _context.DiscountDiscountTypes.RemoveRange(existingDiscount.DiscountDiscountTypes);
                foreach (var type in discount.DiscountDiscountTypes)
                {
                    type.DiscountId = existingDiscount.DiscountId;
                    _context.DiscountDiscountTypes.Add(type);
                }

                await _context.SaveChangesAsync();
                return existingDiscount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating discount");
                throw;
            }
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            try
            {
                var discount = await _context.Discounts
                    .Include(d => d.DiscountDiscountTypes)
                    .FirstOrDefaultAsync(d => d.DiscountId == id);

                if (discount == null)
                    return false;

                // Check if discount has any associated bookings
                var hasBookings = await _context.Bookings
                    .AnyAsync(b => b.DiscountId == id);

                if (hasBookings)
                    throw new InvalidOperationException("Cannot delete discount with existing bookings");

                _context.DiscountDiscountTypes.RemoveRange(discount.DiscountDiscountTypes);
                _context.Discounts.Remove(discount);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting discount");
                throw;
            }
        }

        public async Task<bool> ValidateDiscountAsync(string couponCode)
        {
            try
            {
                var discount = await _context.Discounts
                    .FirstOrDefaultAsync(d => d.CouponCode == couponCode);

                if (discount == null)
                    return false;

                var now = DateOnly.FromDateTime(DateTime.Now);
                return discount.IsActive == true && 
                       discount.StartDate <= now && 
                       discount.EndDate >= now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating discount");
                throw;
            }
        }

        public async Task<bool> UpdateDiscountStatusAsync(int discountId, bool isActive)
        {
            var discount = await _context.Discounts.FindAsync(discountId);
            if (discount == null)
                return false;

            discount.IsActive = isActive;
            discount.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculateDiscountAmountAsync(string couponCode, decimal originalAmount)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.CouponCode == couponCode);

            if (discount == null || !await ValidateDiscountAsync(couponCode))
                return originalAmount;

            return originalAmount * (1 - discount.DiscountValue / 100);
        }

        public Discount GetDiscountByCode(string couponCode)
        {
            return _context.Discounts
                .Include(d => d.DiscountDiscountTypes)
                    .ThenInclude(ddt => ddt.DiscountType)
                .FirstOrDefault(d => d.CouponCode == couponCode);
        }

        public Discount GetDiscountById(int id)
        {
            return _context.Discounts
                .Include(d => d.DiscountDiscountTypes)
                    .ThenInclude(ddt => ddt.DiscountType)
                .FirstOrDefault(d => d.DiscountId == id);
        }
    }
} 