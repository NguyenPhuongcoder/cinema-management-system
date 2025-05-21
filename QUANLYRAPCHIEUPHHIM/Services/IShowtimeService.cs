using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IShowtimeService
    {
        IEnumerable<Movie> GetAllMovies();
        IEnumerable<Showtime> GetShowtimesByMovie(int movieId);
        Movie GetMovieById(int movieId);
        Showtime GetShowtimeById(int showtimeId);
        Task<IEnumerable<Showtime>> GetShowtimesAsync(
            int? movieId = null,
            int? roomId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            int page = 1,
            int pageSize = 10
        );

        Task<int> CountShowtimesAsync(
            int? movieId = null,
            int? roomId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            decimal? minPrice = null,
            decimal? maxPrice = null
        );

        Task<Showtime> GetShowtimeByIdAsync(int id);
        Task<Showtime> CreateShowtimeAsync(Showtime showtime);
        Task<Showtime> UpdateShowtimeAsync(Showtime showtime);
        Task<bool> DeleteShowtimeAsync(int id);
        Task<bool> ValidateShowtimeAsync(Showtime showtime);
        Task<decimal> CalculateTicketPriceAsync(int showtimeId, int seatId);
        Task<IEnumerable<Showtime>> GetUpcomingShowtimesAsync(int count = 10);
        Task<IEnumerable<Showtime>> GetShowtimesByMovieAsync(int movieId);
        Task<IEnumerable<Showtime>> GetShowtimesByRoomAsync(int roomId);
        Task<IEnumerable<Showtime>> GetShowtimesByDateAsync(DateTime date);
        Task<IEnumerable<Showtime>> GetShowtimesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
} 