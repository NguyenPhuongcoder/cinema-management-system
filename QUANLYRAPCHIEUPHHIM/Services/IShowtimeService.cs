using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IShowtimeService
    {
        // Lấy danh sách suất chiếu
        Task<IEnumerable<Showtime>> GetAllShowtimesAsync();
        
        // Lấy thông tin chi tiết của một suất chiếu
        Task<Showtime> GetShowtimeByIdAsync(int id);
        
        // Lấy danh sách suất chiếu theo phim
        Task<IEnumerable<Showtime>> GetShowtimesByMovieAsync(int movieId);
        
        // Lấy danh sách suất chiếu theo phòng chiếu
        Task<IEnumerable<Showtime>> GetShowtimesByRoomAsync(int roomId);
        
        // Lấy danh sách suất chiếu theo ngày
        Task<IEnumerable<Showtime>> GetShowtimesByDateAsync(DateTime date);
        
        // Lấy danh sách suất chiếu theo khoảng thời gian
        Task<IEnumerable<Showtime>> GetShowtimesByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        // Tạo suất chiếu mới
        Task<Showtime> CreateShowtimeAsync(Showtime showtime);
        
        // Cập nhật suất chiếu
        Task<Showtime> UpdateShowtimeAsync(Showtime showtime);
        
        // Cập nhật hệ số giá
        Task<Showtime> UpdatePriceModifierAsync(int showtimeId, decimal priceModifier);
        
        // Xóa suất chiếu
        Task<bool> DeleteShowtimeAsync(int id);
        
        // Kiểm tra xem suất chiếu có tồn tại không
        Task<bool> ShowtimeExistsAsync(int id);
        
        // Kiểm tra xem phòng chiếu có trống trong khoảng thời gian không
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startTime, DateTime endTime);

        // Tính giá vé dựa trên suất chiếu và ghế
        Task<decimal> CalculateTicketPriceAsync(int showtimeId, int seatId);
    }
} 