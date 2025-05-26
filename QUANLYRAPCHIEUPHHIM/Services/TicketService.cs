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
    public class TicketService : ITicketService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<TicketService> _logger;

        public TicketService(CinemaDbContext context, ILogger<TicketService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.User)
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(t => t.Seat)
                    .FirstOrDefaultAsync(t => t.TicketId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ticket by id {TicketId}", id);
                throw;
            }
        }

        public async Task<Ticket> GetTicketByCodeAsync(string ticketCode)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.User)
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(t => t.Seat)
                    .FirstOrDefaultAsync(t => t.TicketCode == ticketCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ticket by code {TicketCode}", ticketCode);
                throw;
            }
        }

        public async Task<bool> ConfirmTicketAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);

                if (ticket == null)
                {
                    return false;
                }

                if (ticket.TicketStatus == "confirmed")
                {
                    throw new InvalidOperationException("Ticket is already confirmed");
                }

                if (ticket.TicketStatus == "cancelled")
                {
                    throw new InvalidOperationException("Cannot confirm a cancelled ticket");
                }

                ticket.TicketStatus = "confirmed";
                ticket.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming ticket {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<bool> CancelTicketAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.Showtime)
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);

                if (ticket == null)
                {
                    return false;
                }

                if (ticket.TicketStatus == "cancelled")
                {
                    throw new InvalidOperationException("Ticket is already cancelled");
                }

                // Check if showtime has already started
                if (ticket.Showtime.StartTime <= DateTime.Now)
                {
                    throw new InvalidOperationException("Cannot cancel ticket for a showtime that has already started");
                }

                ticket.TicketStatus = "cancelled";
                ticket.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling ticket {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByShowtimeAsync(int showtimeId)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.Booking)
                        .ThenInclude(b => b.User)
                    .Include(t => t.Seat)
                    .Where(t => t.ShowtimeId == showtimeId)
                    .OrderBy(t => t.Seat.RowLetter)
                    .ThenBy(t => t.Seat.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tickets by showtime {ShowtimeId}", showtimeId);
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                    .Include(t => t.Seat)
                    .Include(t => t.Booking)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all tickets");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                    .Include(t => t.Seat)
                    .Include(t => t.Booking)
                    .Where(t => t.TicketStatus == status)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting tickets with status {status}");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                    .Include(t => t.Seat)
                    .Include(t => t.Booking)
                    .Where(t => t.Booking.UserId == userId)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting tickets for user {userId}");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByBookingAsync(int bookingId)
        {
            try
            {
                return await _context.Tickets
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Movie)
                    .Include(t => t.Showtime)
                        .ThenInclude(s => s.Room)
                            .ThenInclude(r => r.Cinema)
                    .Include(t => t.Seat)
                    .Include(t => t.Booking)
                    .Where(t => t.BookingId == bookingId)
                    .OrderBy(t => t.Seat.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting tickets for booking {bookingId}");
                throw;
            }
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            try
            {
                // Kiểm tra xem ghế đã được đặt chưa
                var existingTicket = await _context.Tickets
                    .AnyAsync(t => t.ShowtimeId == ticket.ShowtimeId && 
                                 t.SeatId == ticket.SeatId);

                if (existingTicket)
                {
                    throw new InvalidOperationException("Seat is already booked for this showtime");
                }

                ticket.CreatedAt = DateTime.Now;
                ticket.TicketStatus = "pending";
                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ticket");
                throw;
            }
        }

        public async Task<Ticket> UpdateTicketStatusAsync(int ticketId, string status)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    throw new KeyNotFoundException($"Ticket with ID {ticketId} not found");
                }

                ticket.TicketStatus = status;
                ticket.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating ticket status for ticket {ticketId}");
                throw;
            }
        }

        public async Task<Ticket> UpdateScanDatetimeAsync(int ticketId, DateTime scanDatetime)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(ticketId);
                if (ticket == null)
                {
                    throw new KeyNotFoundException($"Ticket with ID {ticketId} not found");
                }

                ticket.ScanDatetime = scanDatetime;
                ticket.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating scan datetime for ticket {ticketId}");
                throw;
            }
        }

        public async Task<bool> TicketExistsAsync(int id)
        {
            return await _context.Tickets.AnyAsync(t => t.TicketId == id);
        }
    }
} 