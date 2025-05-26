using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public class SeatService
    {
        private readonly CinemaDbcontext _context;

        public SeatService(CinemaDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> GenerateSeatsForRoom(int roomId)
        {
            var room = await _context.Rooms
                .Include(r => r.Seats)
                .FirstOrDefaultAsync(r => r.RoomId == roomId);

            if (room == null)
                return false;

            // Check if seats already exist
            if (room.Seats.Any())
                return false;

            // Get default seat type (assuming ID 1 is standard)
            var defaultSeatType = await _context.SeatTypes.FirstOrDefaultAsync();
            if (defaultSeatType == null)
                return false;

            var seats = new List<Seat>();
            int currentSeatCount = 0;
            char currentRow = 'A';

            // Generate seats until we reach room capacity
            while (currentSeatCount < room.Capacity)
            {
                // Generate 10 seats per row (you can adjust this number)
                for (int seatNumber = 1; seatNumber <= 10 && currentSeatCount < room.Capacity; seatNumber++)
                {
                    seats.Add(new Seat
                    {
                        RoomId = roomId,
                        SeatTypeId = defaultSeatType.SeatTypeId,
                        RowLetter = currentRow.ToString(),
                        SeatNumber = seatNumber,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                    currentSeatCount++;
                }
                currentRow++;
            }

            await _context.Seats.AddRangeAsync(seats);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Seat>> GetSeatsByRoom(int roomId)
        {
            return await _context.Seats
                .Include(s => s.SeatType)
                .Where(s => s.RoomId == roomId)
                .OrderBy(s => s.RowLetter)
                .ThenBy(s => s.SeatNumber)
                .ToListAsync();
        }

        public async Task<bool> UpdateSeatType(int seatId, int newSeatTypeId)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            if (seat == null)
                return false;

            var seatType = await _context.SeatTypes.FindAsync(newSeatTypeId);
            if (seatType == null)
                return false;

            seat.SeatTypeId = newSeatTypeId;
            seat.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
} 