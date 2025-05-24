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

        public async Task<Ticket> ConfirmTicketAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);

                if (ticket == null)
                    throw new ArgumentException("Ticket not found");

                if (ticket.TicketStatus == "confirmed")
                    throw new InvalidOperationException("Ticket is already confirmed");

                if (ticket.TicketStatus == "cancelled")
                    throw new InvalidOperationException("Cannot confirm a cancelled ticket");

                ticket.TicketStatus = "confirmed";
                ticket.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming ticket {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<Ticket> CancelTicketAsync(int ticketId)
        {
            try
            {
                var ticket = await _context.Tickets
                    .FirstOrDefaultAsync(t => t.TicketId == ticketId);

                if (ticket == null)
                    throw new ArgumentException("Ticket not found");

                if (ticket.TicketStatus == "cancelled")
                    throw new InvalidOperationException("Ticket is already cancelled");

                // Check if showtime has already started
                var showtime = await _context.Showtimes
                    .FirstOrDefaultAsync(s => s.ShowtimeId == ticket.ShowtimeId);

                if (showtime == null)
                    throw new ArgumentException("Showtime not found");

                if (showtime.StartTime <= DateTime.Now)
                    throw new InvalidOperationException("Cannot cancel ticket for a showtime that has already started");

                ticket.TicketStatus = "cancelled";
                ticket.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return ticket;
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
    }
} 