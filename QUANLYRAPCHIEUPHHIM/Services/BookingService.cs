using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public class BookingService : IBookingService
    {
        private readonly CinemaDbContext _context;

        public BookingService(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(
            string customerName = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                .Include(b => b.BookingBookingStatuses)
                    .ThenInclude(bbs => bbs.BookingStatus)
                .Include(b => b.Discount)
                .AsQueryable();

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(b => b.User.FullName.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(customerPhone))
            {
                query = query.Where(b => b.User.Phone.Contains(customerPhone));
            }

            if (movieId.HasValue)
            {
                query = query.Where(b => b.Tickets.Any(t => t.Showtime.MovieId == movieId.Value));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.BookingBookingStatuses.Any(bbs => bbs.BookingStatus.BookingStatusName == status));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= toDate.Value);
            }

            return await query
                .OrderByDescending(b => b.BookingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountBookingsAsync(
            string customerName = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                .Include(b => b.BookingBookingStatuses)
                    .ThenInclude(bbs => bbs.BookingStatus)
                .AsQueryable();

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(b => b.User.FullName.Contains(customerName));
            }

            if (!string.IsNullOrEmpty(customerPhone))
            {
                query = query.Where(b => b.User.Phone.Contains(customerPhone));
            }

            if (movieId.HasValue)
            {
                query = query.Where(b => b.Tickets.Any(t => t.Showtime.MovieId == movieId.Value));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(b => b.BookingBookingStatuses.Any(bbs => bbs.BookingStatus.BookingStatusName == status));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= toDate.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Seat)
                .Include(b => b.BookingBookingStatuses)
                    .ThenInclude(bbs => bbs.BookingStatus)
                .Include(b => b.Discount)
                .FirstOrDefaultAsync(b => b.BookingId == id);
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            booking.CreatedAt = DateTime.Now;
            booking.UpdatedAt = DateTime.Now;
            booking.BookingDate = DateTime.Now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            booking.UpdatedAt = DateTime.Now;
            _context.Entry(booking).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingBookingStatuses)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return false;

            var cancelledStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.BookingStatusName == "cancelled");

            if (cancelledStatus == null)
                return false;

            booking.BookingBookingStatuses.Add(new BookingBookingStatus
            {
                BookingId = booking.BookingId,
                BookingStatusId = cancelledStatus.BookingStatusId
            });

            booking.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingBookingStatuses)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return false;

            var newStatus = await _context.BookingStatuses
                .FirstOrDefaultAsync(s => s.BookingStatusName == status);

            if (newStatus == null)
                return false;

            booking.BookingBookingStatuses.Add(new BookingBookingStatus
            {
                BookingId = booking.BookingId,
                BookingStatusId = newStatus.BookingStatusId,
            });

            booking.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> CalculateTotalAmountAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Tickets)
                .Include(b => b.Discount)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return 0;

            var totalAmount = booking.Tickets.Sum(t => t.Price);

            if (booking.Discount != null)
            {
                totalAmount = totalAmount * (1 - booking.Discount.DiscountValue / 100);
            }

            return totalAmount;
        }

        public async Task<bool> ValidateBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Tickets)
                    .ThenInclude(t => t.Showtime)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
                return false;

            // Kiểm tra xem tất cả các suất chiếu có còn trong tương lai không
            var now = DateTime.Now;
            if (booking.Tickets.Any(t => t.Showtime.StartTime <= now))
                return false;

            // Kiểm tra xem tất cả các vé có trạng thái hợp lệ không
            if (booking.Tickets.Any(t => t.TicketStatus == "cancelled"))
                return false;

            return true;
        }
    }
} 