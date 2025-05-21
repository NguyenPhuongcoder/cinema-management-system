using Microsoft.EntityFrameworkCore;
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

        public DiscountService(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetDiscountsAsync(
            string discountName = null,
            string couponCode = null,
            string discountType = null,
            bool? isActive = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minValue = null,
            decimal? maxValue = null,
            int page = 1,
            int pageSize = 10)
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

        public async Task<int> CountDiscountsAsync(
            string discountName = null,
            string couponCode = null,
            string discountType = null,
            bool? isActive = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minValue = null,
            decimal? maxValue = null)
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

            return await query.CountAsync();
        }

        public async Task<Discount> GetDiscountByIdAsync(int id)
        {
            return await _context.Discounts
                .Include(d => d.DiscountDiscountTypes)
                    .ThenInclude(ddt => ddt.DiscountType)
                .FirstOrDefaultAsync(d => d.DiscountId == id);
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
            discount.CreatedAt = DateTime.Now;
            discount.UpdatedAt = DateTime.Now;

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<Discount> UpdateDiscountAsync(Discount discount)
        {
            discount.UpdatedAt = DateTime.Now;
            _context.Entry(discount).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return discount;
        }

        public async Task<bool> DeleteDiscountAsync(int id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
                return false;

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateDiscountAsync(string couponCode)
        {
            var discount = await _context.Discounts
                .FirstOrDefaultAsync(d => d.CouponCode == couponCode);

            if (discount == null)
                return false;

            var now = DateTime.Now;
            if (!discount.IsActive || discount.StartDate > now || discount.EndDate < now)
                return false;

            return true;
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