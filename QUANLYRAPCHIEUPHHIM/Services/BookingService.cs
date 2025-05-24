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
    public class BookingService : IBookingService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<BookingService> _logger;
        private readonly IShowtimeService _showtimeService;
        private readonly IPaymentService _paymentService;

        public BookingService(
            CinemaDbContext context,
            ILogger<BookingService> logger,
            IShowtimeService showtimeService,
            IPaymentService paymentService)
        {
            _context = context;
            _logger = logger;
            _showtimeService = showtimeService;
            _paymentService = paymentService;
        }

        public async Task<IEnumerable<Booking>> GetBookingsAsync(
            int? userId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? status = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.BookingBookingStatuses)
                        .ThenInclude(bbs => bbs.BookingStatus)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
                            .ThenInclude(s => s.Movie)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                    .Include(b => b.Discount)
                    .AsQueryable();

                if (userId.HasValue)
                {
                    // Check both User and ApplicationUser
                    query = query.Where(b => b.UserId == userId.Value);
                }

                if (fromDate.HasValue)
                    query = query.Where(b => b.BookingDate >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(b => b.BookingDate <= toDate.Value);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(b => b.BookingBookingStatuses
                        .Any(bbs => bbs.BookingStatus.BookingStatusName == status));

                return await query
                    .OrderByDescending(b => b.BookingDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bookings");
                throw;
            }
        }

        public async Task<int> CountBookingsAsync(
            string? customerName = null,
            string? customerPhone = null,
            int? movieId = null,
            string? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            try
            {
                var query = _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.BookingBookingStatuses)
                        .ThenInclude(bbs => bbs.BookingStatus)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
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
                    query = query.Where(b => b.Tickets
                        .Any(t => t.Showtime.MovieId == movieId.Value));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(b => b.BookingBookingStatuses
                        .Any(bbs => bbs.BookingStatus.BookingStatusName == status));
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting bookings");
                throw;
            }
        }

        public async Task<Booking> GetBookingByIdAsync(int id)
        {
            try
            {
                return await _context.Bookings
                    .Include(b => b.User)
                    .Include(b => b.BookingBookingStatuses)
                        .ThenInclude(bbs => bbs.BookingStatus)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
                            .ThenInclude(s => s.Movie)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Seat)
                    .Include(b => b.Discount)
                    .FirstOrDefaultAsync(b => b.BookingId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting booking by id {BookingId}", id);
                throw;
            }
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            try
            {
                // Validate booking data
                if (booking.UserId <= 0)
                    throw new ArgumentException("User ID is required");

                if (!booking.Tickets.Any())
                    throw new ArgumentException("At least one ticket is required");

                // Check if user exists
                var user = await _context.Users.FindAsync(booking.UserId);
                if (user == null)
                    throw new ArgumentException("User not found");

                // Validate tickets
                foreach (var ticket in booking.Tickets)
                {
                    // Check if showtime exists and is valid
                    var showtime = await _context.Showtimes
                        .Include(s => s.Movie)
                        .FirstOrDefaultAsync(s => s.ShowtimeId == ticket.ShowtimeId);
                    if (showtime == null)
                        throw new ArgumentException($"Showtime {ticket.ShowtimeId} not found");
                    if (showtime.StartTime <= DateTime.Now)
                        throw new ArgumentException($"Showtime {ticket.ShowtimeId} has already started");

                    // Check if seat exists and is available
                    var seat = await _context.Seats
                        .Include(s => s.SeatType)
                        .FirstOrDefaultAsync(s => s.SeatId == ticket.SeatId);
                    if (seat == null)
                        throw new ArgumentException($"Seat {ticket.SeatId} not found");

                    // Check if seat is already booked
                    var existingTicket = await _context.Tickets
                        .FirstOrDefaultAsync(t => t.ShowtimeId == ticket.ShowtimeId && 
                                                t.SeatId == ticket.SeatId);
                    if (existingTicket != null)
                        throw new ArgumentException($"Seat {ticket.SeatId} is already booked for this showtime");

                    // Calculate ticket price
                    ticket.Price = await _showtimeService.CalculateTicketPriceAsync(
                        showtime.ShowtimeId, seat.SeatTypeId);
                }

                // Calculate total amount
                booking.TotalAmount = booking.Tickets.Sum(t => t.Price);

                // Apply discount if any
                if (booking.DiscountId.HasValue)
                {
                    var discount = await _context.Discounts
                        .FirstOrDefaultAsync(d => d.DiscountId == booking.DiscountId.Value);
                    if (discount != null && discount.IsActive == true)
                    {
                        booking.TotalAmount -= booking.TotalAmount * discount.DiscountValue / 100m;
                    }
                }

                // Set booking status to Pending
                var pendingStatus = await _context.BookingStatuses
                    .FirstOrDefaultAsync(s => s.BookingStatusName == "Pending");
                if (pendingStatus == null)
                    throw new InvalidOperationException("Pending status not found");

                booking.BookingDate = DateTime.Now;
                booking.CreatedAt = DateTime.Now;
                booking.UpdatedAt = DateTime.Now;
                booking.PaymentDueDate = DateTime.Now.AddHours(24); // 24 hours to complete payment

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                // Add booking status
                var bookingStatus = new BookingBookingStatus
                {
                    BookingId = booking.BookingId,
                    BookingStatusId = pendingStatus.BookingStatusId
                };
                _context.BookingBookingStatuses.Add(bookingStatus);
                await _context.SaveChangesAsync();

                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking");
                throw;
            }
        }

        public async Task<Booking> UpdateBookingAsync(Booking booking)
        {
            try
            {
                var existingBooking = await _context.Bookings
                    .Include(b => b.Tickets)
                    .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);
                if (existingBooking == null)
                    throw new ArgumentException("Booking not found");

                // Check if booking can be updated
                var hasTickets = await _context.Tickets
                    .AnyAsync(t => t.BookingId == booking.BookingId);
                if (hasTickets)
                    throw new InvalidOperationException("Cannot update booking with existing tickets");

                // Update booking properties
                existingBooking.DiscountId = booking.DiscountId;
                existingBooking.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingBooking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking {BookingId}", booking.BookingId);
                throw;
            }
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.BookingBookingStatuses)
                        .ThenInclude(bbs => bbs.BookingStatus)
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                if (booking == null)
                    return false;

                // Check if booking can be cancelled
                var currentStatus = booking.BookingBookingStatuses
                    .OrderByDescending(bbs => bbs.BookingStatusId)
                    .FirstOrDefault()?.BookingStatus.BookingStatusName;
                if (currentStatus == "Cancelled" || currentStatus == "Completed")
                    return false;

                // Check if showtime has started
                if (booking.Tickets.Any(t => t.Showtime.StartTime <= DateTime.Now))
                    return false;

                // Add cancelled status
                var cancelledStatus = await _context.BookingStatuses
                    .FirstOrDefaultAsync(s => s.BookingStatusName == "Cancelled");
                if (cancelledStatus == null)
                    throw new InvalidOperationException("Cancelled status not found");

                var bookingStatus = new BookingBookingStatus
                {
                    BookingId = booking.BookingId,
                    BookingStatusId = cancelledStatus.BookingStatusId
                };
                _context.BookingBookingStatuses.Add(bookingStatus);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
                throw;
            }
        }

        public async Task<Booking> UpdateBookingStatusAsync(int bookingId, string status)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.BookingBookingStatuses)
                        .ThenInclude(bbs => bbs.BookingStatus)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                if (booking == null)
                    throw new ArgumentException("Booking not found");

                var bookingStatus = await _context.BookingStatuses
                    .FirstOrDefaultAsync(s => s.BookingStatusName == status);
                if (bookingStatus == null)
                    throw new ArgumentException("Invalid status");

                var newStatus = new BookingBookingStatus
                {
                    BookingId = booking.BookingId,
                    BookingStatusId = bookingStatus.BookingStatusId
                };
                _context.BookingBookingStatuses.Add(newStatus);
                await _context.SaveChangesAsync();

                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating booking status {BookingId}", bookingId);
                throw;
            }
        }

        public async Task<decimal> CalculateTotalAmountAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Tickets)
                    .Include(b => b.Discount)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                if (booking == null)
                    throw new ArgumentException("Booking not found");

                var totalAmount = booking.Tickets.Sum(t => t.Price);

                if (booking.Discount != null && booking.Discount.IsActive == true)
                {
                    totalAmount -= totalAmount * booking.Discount.DiscountValue / 100m;
                }

                return totalAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total amount for booking {BookingId}", bookingId);
                throw;
            }
        }

        public async Task<bool> ValidateBookingAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                if (booking == null)
                    return false;

                // Check if any showtime has started
                if (booking.Tickets.Any(t => t.Showtime.StartTime <= DateTime.Now))
                    return false;

                // Check if payment is overdue
                if (booking.PaymentDueDate.HasValue && booking.PaymentDueDate.Value < DateTime.Now)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating booking {BookingId}", bookingId);
                throw;
            }
        }

        public async Task<Payment> ProcessPaymentAsync(int bookingId, int paymentMethodId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Tickets)
                        .ThenInclude(t => t.Showtime)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);
                if (booking == null)
                    throw new ArgumentException("Booking not found");

                // Validate booking
                if (!await ValidateBookingAsync(bookingId))
                    throw new InvalidOperationException("Booking is not valid for payment");

                // Check if payment is overdue
                if (booking.PaymentDueDate.HasValue && booking.PaymentDueDate.Value < DateTime.Now)
                    throw new InvalidOperationException("Payment is overdue");

                // Create payment
                var payment = new Payment
                {
                    BookingId = bookingId,
                    PaymentMethodId = paymentMethodId,
                    Amount = booking.TotalAmount,
                    PaymentDate = DateTime.Now,
                    PaymentStatus = "Completed"
                };

                var result = await _paymentService.CreatePaymentAsync(payment);
                if (result != null)
                {
                    // Update booking status to Completed
                    await UpdateBookingStatusAsync(bookingId, "Completed");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for booking {BookingId}", bookingId);
                throw;
            }
        }
    }
} 