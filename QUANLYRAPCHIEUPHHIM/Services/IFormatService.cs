using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IFormatService
    {
        // Lấy danh sách định dạng phòng chiếu với phân trang và tìm kiếm
        Task<IEnumerable<RoomFormat>> GetFormatsAsync(
            string? formatName = null,
            int page = 1,
            int pageSize = 10);

        // Đếm tổng số định dạng phòng chiếu
        Task<int> CountFormatsAsync(string? formatName = null);

        // Lấy thông tin chi tiết của một định dạng phòng chiếu
        Task<RoomFormat> GetFormatByIdAsync(int id);

        // Tạo định dạng phòng chiếu mới
        Task<RoomFormat> CreateFormatAsync(RoomFormat format);

        // Cập nhật thông tin định dạng phòng chiếu
        Task<RoomFormat> UpdateFormatAsync(RoomFormat format);

        // Xóa định dạng phòng chiếu
        Task<bool> DeleteFormatAsync(int id);
    }
} 