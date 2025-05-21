using Microsoft.EntityFrameworkCore;
using QUANLYRAPCHIEUPHHIM.Data;
using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public class ShowtimeService : IShowtimeService
    {
        private readonly CinemaDbContext _context;

        public ShowtimeService(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesAsync(
            int? movieId = null,
            int? roomId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .AsQueryable();

            if (movieId.HasValue)
            {
                query = query.Where(s => s.MovieId == movieId.Value);
            }

            if (roomId.HasValue)
            {
                query = query.Where(s => s.RoomId == roomId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.StartTime >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(s => s.EndTime <= toDate.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(s => s.PriceModifier >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.PriceModifier <= maxPrice.Value);
            }

            return await query
                .OrderBy(s => s.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountShowtimesAsync(
            int? movieId = null,
            int? roomId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            var query = _context.Showtimes.AsQueryable();

            if (movieId.HasValue)
            {
                query = query.Where(s => s.MovieId == movieId.Value);
            }

            if (roomId.HasValue)
            {
                query = query.Where(s => s.RoomId == roomId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(s => s.StartTime >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(s => s.EndTime <= toDate.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(s => s.PriceModifier >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(s => s.PriceModifier <= maxPrice.Value);
            }

            return await query.CountAsync();
        }

        public async Task<Showtime> GetShowtimeByIdAsync(int id)
        {
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .FirstOrDefaultAsync(s => s.ShowtimeId == id);
        }

        public async Task<Showtime> CreateShowtimeAsync(Showtime showtime)
        {
            showtime.CreatedAt = DateTime.Now;
            showtime.UpdatedAt = DateTime.Now;

            _context.Showtimes.Add(showtime);
            await _context.SaveChangesAsync();
            return showtime;
        }

        public async Task<Showtime> UpdateShowtimeAsync(Showtime showtime)
        {
            showtime.UpdatedAt = DateTime.Now;
            _context.Entry(showtime).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return showtime;
        }

        public async Task<bool> DeleteShowtimeAsync(int id)
        {
            var showtime = await _context.Showtimes.FindAsync(id);
            if (showtime == null)
                return false;

            _context.Showtimes.Remove(showtime);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateShowtimeAsync(Showtime showtime)
        {
            // Check if movie exists
            var movie = await _context.Movies.FindAsync(showtime.MovieId);
            if (movie == null)
                return false;

            // Check if room exists
            var room = await _context.Rooms.FindAsync(showtime.RoomId);
            if (room == null)
                return false;

            // Check if showtime overlaps with existing showtimes in the same room
            var overlappingShowtime = await _context.Showtimes
                .AnyAsync(s => s.RoomId == showtime.RoomId &&
                             s.ShowtimeId != showtime.ShowtimeId &&
                             ((s.StartTime <= showtime.StartTime && s.EndTime > showtime.StartTime) ||
                              (s.StartTime < showtime.EndTime && s.EndTime >= showtime.EndTime) ||
                              (s.StartTime >= showtime.StartTime && s.EndTime <= showtime.EndTime)));

            if (overlappingShowtime)
                return false;

            return true;
        }

        public async Task<decimal> CalculateTicketPriceAsync(int showtimeId, int seatId)
        {
            var showtime = await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Seats)
                        .ThenInclude(seat => seat.SeatType)
                .FirstOrDefaultAsync(s => s.ShowtimeId == showtimeId);

            if (showtime == null)
                throw new ArgumentException("Showtime not found");

            var seat = showtime.Room.Seats.FirstOrDefault(s => s.SeatId == seatId);
            if (seat == null)
                throw new ArgumentException("Seat not found");

            var basePrice = showtime.Movie.BasePrice;
            var seatTypeModifier = seat.SeatType.AdditionalCharge;
            var showtimeModifier = showtime.PriceModifier;

            return basePrice + seatTypeModifier + showtimeModifier;
        }

        public async Task<IEnumerable<Showtime>> GetUpcomingShowtimesAsync(int count = 10)
        {
            var now = DateTime.Now;
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.StartTime > now)
                .OrderBy(s => s.StartTime)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesByMovieAsync(int movieId)
        {
            var now = DateTime.Now;
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.MovieId == movieId && s.StartTime > now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesByRoomAsync(int roomId)
        {
            var now = DateTime.Now;
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.RoomId == roomId && s.StartTime > now)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesByDateAsync(DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.StartTime >= startOfDay && s.StartTime <= endOfDay)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Showtime>> GetShowtimesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.StartTime >= startDate && s.StartTime <= endDate)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            return _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieFormats)
                    .ThenInclude(mf => mf.RoomFormat)
                .OrderByDescending(m => m.ReleaseDate)
                .ToList();
        }

        public IEnumerable<Showtime> GetShowtimesByMovie(int movieId)
        {
            var now = DateTime.Now;
            return _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .Where(s => s.MovieId == movieId && s.StartTime > now)
                .OrderBy(s => s.StartTime)
                .ToList();
        }

        public Movie GetMovieById(int movieId)
        {
            return _context.Movies
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Include(m => m.MovieFormats)
                    .ThenInclude(mf => mf.RoomFormat)
                .FirstOrDefault(m => m.MovieId == movieId);
        }

        public Showtime GetShowtimeById(int showtimeId)
        {
            return _context.Showtimes
                .Include(s => s.Movie)
                .Include(s => s.Room)
                    .ThenInclude(r => r.Cinema)
                .FirstOrDefault(s => s.ShowtimeId == showtimeId);
        }
    }
} 