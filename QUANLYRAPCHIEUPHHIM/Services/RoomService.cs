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
    public class RoomService : IRoomService
    {
        private readonly CinemaDbContext _context;
        private readonly ILogger<RoomService> _logger;

        public RoomService(CinemaDbContext context, ILogger<RoomService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Room>> GetRoomsAsync(
            int? cinemaId = null,
            int? formatId = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Rooms
                    .Include(r => r.Cinema)
                    .Include(r => r.Format)
                    .Include(r => r.Seats)
                        .ThenInclude(s => s.SeatType)
                    .AsQueryable();

                if (cinemaId.HasValue)
                    query = query.Where(r => r.CinemaId == cinemaId.Value);

                if (formatId.HasValue)
                    query = query.Where(r => r.FormatId == formatId.Value);

                return await query
                    .OrderBy(r => r.RoomName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rooms");
                throw;
            }
        }

        public async Task<int> CountRoomsAsync(
            int? cinemaId = null,
            int? formatId = null)
        {
            try
            {
                var query = _context.Rooms.AsQueryable();

                if (cinemaId.HasValue)
                    query = query.Where(r => r.CinemaId == cinemaId.Value);

                if (formatId.HasValue)
                    query = query.Where(r => r.FormatId == formatId.Value);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting rooms");
                throw;
            }
        }

        public async Task<Room> GetRoomByIdAsync(int id)
        {
            try
            {
                return await _context.Rooms
                    .Include(r => r.Cinema)
                    .Include(r => r.Format)
                    .Include(r => r.Seats)
                        .ThenInclude(s => s.SeatType)
                    .FirstOrDefaultAsync(r => r.RoomId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting room by id {RoomId}", id);
                throw;
            }
        }

        public async Task<Room> CreateRoomAsync(Room room)
        {
            try
            {
                // Validate room data
                if (string.IsNullOrEmpty(room.RoomName))
                    throw new ArgumentException("Room name is required");

                if (room.CinemaId <= 0)
                    throw new ArgumentException("Cinema ID is required");

                if (room.FormatId <= 0)
                    throw new ArgumentException("Format ID is required");

                if (room.Capacity <= 0)
                    throw new ArgumentException("Capacity must be greater than 0");

                // Check if room name already exists in the cinema
                var existingRoom = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.CinemaId == room.CinemaId && 
                                            r.RoomName == room.RoomName);

                if (existingRoom != null)
                    throw new ArgumentException("Room name already exists in this cinema");

                room.CreatedAt = DateTime.Now;
                room.UpdatedAt = DateTime.Now;

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                return room;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating room");
                throw;
            }
        }

        public async Task<Room> UpdateRoomAsync(Room room)
        {
            try
            {
                var existingRoom = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.RoomId == room.RoomId);

                if (existingRoom == null)
                    throw new ArgumentException("Room not found");

                // Validate room data
                if (string.IsNullOrEmpty(room.RoomName))
                    throw new ArgumentException("Room name is required");

                if (room.CinemaId <= 0)
                    throw new ArgumentException("Cinema ID is required");

                if (room.FormatId <= 0)
                    throw new ArgumentException("Format ID is required");

                if (room.Capacity <= 0)
                    throw new ArgumentException("Capacity must be greater than 0");

                // Check if room name already exists in the cinema (excluding current room)
                var duplicateRoom = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.CinemaId == room.CinemaId && 
                                            r.RoomName == room.RoomName &&
                                            r.RoomId != room.RoomId);

                if (duplicateRoom != null)
                    throw new ArgumentException("Room name already exists in this cinema");

                // Check if room has any showtimes
                var hasShowtimes = await _context.Showtimes
                    .AnyAsync(s => s.RoomId == room.RoomId);

                if (hasShowtimes)
                    throw new InvalidOperationException("Cannot update room with existing showtimes");

                existingRoom.RoomName = room.RoomName;
                existingRoom.CinemaId = room.CinemaId;
                existingRoom.FormatId = room.FormatId;
                existingRoom.Capacity = room.Capacity;
                existingRoom.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return existingRoom;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating room");
                throw;
            }
        }

        public async Task<bool> DeleteRoomAsync(int id)
        {
            try
            {
                var room = await _context.Rooms
                    .Include(r => r.Showtimes)
                    .Include(r => r.Seats)
                        .ThenInclude(s => s.Tickets)
                    .FirstOrDefaultAsync(r => r.RoomId == id);

                if (room == null)
                    return false;

                // Check if room has any showtimes
                if (room.Showtimes.Any())
                    throw new InvalidOperationException("Cannot delete room with existing showtimes");

                // Check if room has any seats with tickets
                if (room.Seats.Any(s => s.Tickets.Any()))
                    throw new InvalidOperationException("Cannot delete room with seats that have tickets");

                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting room");
                throw;
            }
        }

        public async Task<IEnumerable<Room>> GetRoomsByCinemaAsync(int cinemaId)
        {
            try
            {
                return await _context.Rooms
                    .Include(r => r.Format)
                    .Include(r => r.Seats)
                        .ThenInclude(s => s.SeatType)
                    .Where(r => r.CinemaId == cinemaId)
                    .OrderBy(r => r.RoomName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rooms by cinema {CinemaId}", cinemaId);
                throw;
            }
        }

        public async Task<IEnumerable<Room>> GetRoomsByFormatAsync(int formatId)
        {
            try
            {
                return await _context.Rooms
                    .Include(r => r.Cinema)
                    .Include(r => r.Seats)
                        .ThenInclude(s => s.SeatType)
                    .Where(r => r.FormatId == formatId)
                    .OrderBy(r => r.RoomName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rooms by format {FormatId}", formatId);
                throw;
            }
        }
    }
} 