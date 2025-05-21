using Microsoft.EntityFrameworkCore;
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

        public PaymentService(CinemaDbContext context)
        {
            _context = context;
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
            int pageSize = 10)
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

            return await query
                .OrderByDescending(p => p.PaymentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
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
            return await _context.Payments
                .Include(p => p.Booking)
                    .ThenInclude(b => b.User)
                .Include(p => p.PaymentMethod)
                .FirstOrDefaultAsync(p => p.PaymentId == id);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            payment.CreatedAt = DateTime.Now;
            payment.UpdatedAt = DateTime.Now;
            payment.PaymentDate = DateTime.Now;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            payment.UpdatedAt = DateTime.Now;
            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return payment;
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
    }
} 