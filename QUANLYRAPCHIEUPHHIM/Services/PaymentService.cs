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
    public class PaymentService : IPaymentService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(CinemaDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Payment CreatePayment(Payment payment)
        {
            payment.CreatedAt = DateTime.Now;
            payment.UpdatedAt = DateTime.Now;
            payment.PaymentDate = DateTime.Now;

            _context.Payments.Add(payment);
            _context.SaveChanges();
            return payment;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsAsync(
            int? bookingId = null,
            int? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int? paymentMethodId = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Payments
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.User)
                    .Include(p => p.PaymentMethod)
                    .AsQueryable();

                if (bookingId.HasValue)
                    query = query.Where(p => p.BookingId == bookingId.Value);

                if (userId.HasValue)
                    query = query.Where(p => p.Booking.UserId == userId.Value);

                if (fromDate.HasValue)
                    query = query.Where(p => p.PaymentDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(p => p.PaymentDate <= toDate.Value);

                if (paymentMethodId.HasValue)
                    query = query.Where(p => p.PaymentMethodId == paymentMethodId.Value);

                return await query
                    .OrderByDescending(p => p.PaymentDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payments");
                throw;
            }
        }

        public async Task<int> CountPaymentsAsync(
            string customerName = null,
            string customerPhone = null,
            int? bookingId = null,
            string paymentMethod = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minAmount = null,
            decimal? maxAmount = null)
        {
            var query = _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.PaymentMethod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(p => p.Booking.User.FullName.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(customerPhone))
            {
                query = query.Where(p => p.Booking.User.Phone.Contains(customerPhone));
            }

            if (bookingId.HasValue)
            {
                query = query.Where(p => p.BookingId == bookingId.Value);
            }

            if (!string.IsNullOrEmpty(paymentMethod))
            {
                query = query.Where(p => p.PaymentMethod.MethodName == paymentMethod);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.PaymentStatus == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= toDate.Value);
            }

            if (minAmount.HasValue)
            {
                query = query.Where(p => p.Amount >= minAmount.Value);
            }

            if (maxAmount.HasValue)
            {
                query = query.Where(p => p.Amount <= maxAmount.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Payment> GetPaymentByIdAsync(int id)
        {
            try
            {
                return await _context.Payments
                    .Include(p => p.Booking)
                        .ThenInclude(b => b.User)
                    .Include(p => p.PaymentMethod)
                    .FirstOrDefaultAsync(p => p.PaymentId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment by id {PaymentId}", id);
                throw;
            }
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            try
            {
                // Validate booking exists
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == payment.BookingId);
                if (booking == null)
                    throw new ArgumentException("Booking not found");

                // Validate payment method exists
                var paymentMethod = await _context.PaymentMethods
                    .FirstOrDefaultAsync(pm => pm.PaymentMethodId == payment.PaymentMethodId);
                if (paymentMethod == null)
                    throw new ArgumentException("Payment method not found");

                // Validate amount matches booking total
                if (payment.Amount != booking.TotalAmount)
                    throw new ArgumentException("Payment amount does not match booking total");

                payment.PaymentDate = DateTime.UtcNow;
                payment.PaymentStatus = "Completed";
                payment.CreatedAt = DateTime.UtcNow;
                payment.UpdatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                throw;
            }
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            try
            {
                var existingPayment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.PaymentId == payment.PaymentId);
                if (existingPayment == null)
                    throw new ArgumentException("Payment not found");

                // Only allow updating payment method
                existingPayment.PaymentMethodId = payment.PaymentMethodId;
                existingPayment.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingPayment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment {PaymentId}", payment.PaymentId);
                throw;
            }
        }

        public async Task<bool> UpdatePaymentStatusAsync(int paymentId, string status)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
                return false;

            payment.PaymentStatus = status;
            payment.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetTotalPaymentsByDateAsync(DateTime date)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate == date.Date && p.PaymentStatus == "completed")
                .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate && p.PaymentStatus == "completed")
                .SumAsync(p => p.Amount);
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            try
            {
                var payment = await _context.Payments
                    .FirstOrDefaultAsync(p => p.PaymentId == id);
                if (payment == null)
                    return false;

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment {PaymentId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethodsAsync()
        {
            try
            {
                return await _context.PaymentMethods
                    .OrderBy(pm => pm.MethodName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment methods");
                throw;
            }
        }

        public async Task<PaymentMethod> GetPaymentMethodByIdAsync(int id)
        {
            try
            {
                return await _context.PaymentMethods
                    .FirstOrDefaultAsync(pm => pm.PaymentMethodId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment method by id {PaymentMethodId}", id);
                throw;
            }
        }
    }
} 