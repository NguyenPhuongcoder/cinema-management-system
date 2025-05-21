using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IDiscountService
    {
        Discount GetDiscountByCode(string couponCode);
        Discount GetDiscountById(int id);
        Task<IEnumerable<Discount>> GetDiscountsAsync(
            string discountName = null,
            string couponCode = null,
            string discountType = null,
            bool? isActive = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minValue = null,
            decimal? maxValue = null,
            int page = 1,
            int pageSize = 10
        );

        Task<int> CountDiscountsAsync(
            string discountName = null,
            string couponCode = null,
            string discountType = null,
            bool? isActive = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minValue = null,
            decimal? maxValue = null
        );

        Task<Discount> GetDiscountByIdAsync(int id);
        Task<Discount> GetDiscountByCouponCodeAsync(string couponCode);
        Task<Discount> CreateDiscountAsync(Discount discount);
        Task<Discount> UpdateDiscountAsync(Discount discount);
        Task<bool> DeleteDiscountAsync(int id);
        Task<bool> ValidateDiscountAsync(string couponCode);
        Task<bool> UpdateDiscountStatusAsync(int discountId, bool isActive);
        Task<decimal> CalculateDiscountAmountAsync(string couponCode, decimal originalAmount);
    }
} 