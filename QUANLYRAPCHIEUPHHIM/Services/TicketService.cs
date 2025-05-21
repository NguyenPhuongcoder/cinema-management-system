using Microsoft.EntityFrameworkCore;
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

        public TicketService(CinemaDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Ticket> GetAllTickets()
        {
            return _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.User)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .ToList();
        }

        public Ticket GetTicketById(int id)
        {
            return _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.User)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .FirstOrDefault(t => t.TicketId == id);
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.User)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }

        public void CreateTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }

        public void UpdateTicket(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            _context.SaveChanges();
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public void DeleteTicket(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
            }
        }

        public IEnumerable<int> GetBookedSeats(int showtimeId)
        {
            return _context.Tickets
                .Where(t => t.ShowtimeId == showtimeId && t.TicketStatus != "cancelled")
                .Select(t => t.SeatId)
                .ToList();
        }

        public async Task<IEnumerable<Ticket>> GetTicketsAsync(
            string ticketCode = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.User)
                .Include(t => t.Showtime)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .AsQueryable();

            if (!string.IsNullOrEmpty(ticketCode))
            {
                query = query.Where(t => t.TicketCode.Contains(ticketCode));
            }

            if (!string.IsNullOrEmpty(customerPhone))
            {
                query = query.Where(t => t.Booking.User.Phone.Contains(customerPhone));
            }

            if (movieId.HasValue)
            {
                query = query.Where(t => t.Showtime.MovieId == movieId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.TicketStatus == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountTicketsAsync(
            string ticketCode = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = _context.Tickets
                .Include(t => t.Booking)
                    .ThenInclude(b => b.User)
                .Include(t => t.Showtime)
                .AsQueryable();

            if (!string.IsNullOrEmpty(ticketCode))
            {
                query = query.Where(t => t.TicketCode.Contains(ticketCode));
            }

            if (!string.IsNullOrEmpty(customerPhone))
            {
                query = query.Where(t => t.Booking.User.Phone.Contains(customerPhone));
            }

            if (movieId.HasValue)
            {
                query = query.Where(t => t.Showtime.MovieId == movieId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.TicketStatus == status);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            return await query.CountAsync();
        }
    }
} 