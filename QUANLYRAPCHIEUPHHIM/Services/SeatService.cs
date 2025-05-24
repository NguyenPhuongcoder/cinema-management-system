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
    public class SeatService : ISeatService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<SeatService> _logger;

        public SeatService(CinemaDbContext context, ILogger<SeatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Seat> GetSeatsByRoom(int roomId)
        {
            try
            {
                return _context.Seats
                    .Include(s => s.SeatType)
                    .Where(s => s.RoomId == roomId)
                    .OrderBy(s => s.RowLetter)
                    .ThenBy(s => s.SeatNumber)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seats by room {RoomId}", roomId);
                throw;
            }
        }

        public IEnumerable<Seat> GetSeatsByIds(List<int> seatIds)
        {
            try
            {
                if (seatIds == null || !seatIds.Any())
                    throw new ArgumentException("Seat IDs cannot be null or empty");

                return _context.Seats
                    .Include(s => s.SeatType)
                    .Where(s => seatIds.Contains(s.SeatId))
                    .OrderBy(s => s.RowLetter)
                    .ThenBy(s => s.SeatNumber)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seats by IDs");
                throw;
            }
        }

        public async Task<IEnumerable<Seat>> GetSeatsByRoomAsync(int roomId)
        {
            try
            {
                return await _context.Seats
                    .Include(s => s.SeatType)
                    .Where(s => s.RoomId == roomId)
                    .OrderBy(s => s.RowLetter)
                    .ThenBy(s => s.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seats by room {RoomId}", roomId);
                throw;
            }
        }

        public async Task<IEnumerable<Seat>> GetSeatsByIdsAsync(List<int> seatIds)
        {
            try
            {
                if (seatIds == null || !seatIds.Any())
                    throw new ArgumentException("Seat IDs cannot be null or empty");

                return await _context.Seats
                    .Include(s => s.SeatType)
                    .Where(s => seatIds.Contains(s.SeatId))
                    .OrderBy(s => s.RowLetter)
                    .ThenBy(s => s.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting seats by IDs");
                throw;
            }
        }

        public async Task<Seat> CreateSeatAsync(Seat seat)
        {
            try
            {
                // Validate seat data
                if (string.IsNullOrEmpty(seat.RowLetter))
                    throw new ArgumentException("Row letter is required");

                if (seat.SeatNumber <= 0)
                    throw new ArgumentException("Seat number must be greater than 0");

                if (seat.RoomId <= 0)
                    throw new ArgumentException("Room ID is required");

                if (seat.SeatTypeId <= 0)
                    throw new ArgumentException("Seat type ID is required");

                // Check if seat already exists in the room
                var existingSeat = await _context.Seats
                    .FirstOrDefaultAsync(s => s.RoomId == seat.RoomId && 
                                            s.RowLetter == seat.RowLetter && 
                                            s.SeatNumber == seat.SeatNumber);

                if (existingSeat != null)
                    throw new ArgumentException("Seat already exists in this room");

                seat.CreatedAt = DateTime.Now;
                seat.UpdatedAt = DateTime.Now;

                _context.Seats.Add(seat);
                await _context.SaveChangesAsync();
                return seat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating seat");
                throw;
            }
        }

        public async Task<Seat> UpdateSeatAsync(Seat seat)
        {
            try
            {
                var existingSeat = await _context.Seats
                    .FirstOrDefaultAsync(s => s.SeatId == seat.SeatId);

                if (existingSeat == null)
                    throw new ArgumentException("Seat not found");

                // Validate seat data
                if (string.IsNullOrEmpty(seat.RowLetter))
                    throw new ArgumentException("Row letter is required");

                if (seat.SeatNumber <= 0)
                    throw new ArgumentException("Seat number must be greater than 0");

                if (seat.RoomId <= 0)
                    throw new ArgumentException("Room ID is required");

                if (seat.SeatTypeId <= 0)
                    throw new ArgumentException("Seat type ID is required");

                // Check if seat already exists in the room (excluding current seat)
                var duplicateSeat = await _context.Seats
                    .FirstOrDefaultAsync(s => s.RoomId == seat.RoomId && 
                                            s.RowLetter == seat.RowLetter && 
                                            s.SeatNumber == seat.SeatNumber &&
                                            s.SeatId != seat.SeatId);

                if (duplicateSeat != null)
                    throw new ArgumentException("Seat already exists in this room");

                existingSeat.RowLetter = seat.RowLetter;
                existingSeat.SeatNumber = seat.SeatNumber;
                existingSeat.RoomId = seat.RoomId;
                existingSeat.SeatTypeId = seat.SeatTypeId;
                existingSeat.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingSeat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating seat");
                throw;
            }
        }

        public async Task<bool> DeleteSeatAsync(int id)
        {
            try
            {
                var seat = await _context.Seats
                    .FirstOrDefaultAsync(s => s.SeatId == id);

                if (seat == null)
                    return false;

                // Check if seat has any associated tickets
                var hasTickets = await _context.Tickets
                    .AnyAsync(t => t.SeatId == id);

                if (hasTickets)
                    throw new InvalidOperationException("Cannot delete seat with existing tickets");

                _context.Seats.Remove(seat);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting seat");
                throw;
            }
        }
    }
} 