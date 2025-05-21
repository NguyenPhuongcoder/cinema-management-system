using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IPaymentService
    {
        Payment CreatePayment(Payment payment);
        Task<IEnumerable<Payment>> GetPaymentsAsync(
            string customerName = null,
            string customerPhone = null,
            int? bookingId = null,
            string paymentMethod = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            int page = 1,
            int pageSize = 10
        );

        Task<int> CountPaymentsAsync(
            string customerName = null,
            string customerPhone = null,
            int? bookingId = null,
            string paymentMethod = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minAmount = null,
            decimal? maxAmount = null
        );

        Task<Payment> GetPaymentByIdAsync(int id);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment> UpdatePaymentAsync(Payment payment);
        Task<bool> UpdatePaymentStatusAsync(int paymentId, string status);
        Task<decimal> GetTotalPaymentsByDateAsync(DateTime date);
        Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
} 