using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetBookingsAsync(
            string customerName = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10
        );

        Task<int> CountBookingsAsync(
            string customerName = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null
        );

        Task<Booking> GetBookingByIdAsync(int id);
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking> UpdateBookingAsync(Booking booking);
        Task<bool> CancelBookingAsync(int id);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
        Task<decimal> CalculateTotalAmountAsync(int bookingId);
        Task<bool> ValidateBookingAsync(int bookingId);
    }
} 