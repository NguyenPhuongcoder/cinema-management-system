using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IMovieService
    {
        // Lấy danh sách phim với phân trang và tìm kiếm
        Task<IEnumerable<Movie>> GetMoviesAsync(
            string? title = null,
            string? status = null,
            int page = 1,
            int pageSize = 10);

        // Lấy danh sách tất cả phim
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        
        // Lấy thông tin chi tiết của một phim
        Task<Movie> GetMovieByIdAsync(int id);
        
        // Tạo phim mới
        Task<Movie> CreateMovieAsync(Movie movie);
        
        // Cập nhật thông tin phim
        Task<Movie> UpdateMovieAsync(Movie movie);
        
        // Xóa phim
        Task<bool> DeleteMovieAsync(int id);
        
        // Kiểm tra xem phim có tồn tại không
        Task<bool> MovieExistsAsync(int id);
    }
} 