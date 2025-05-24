using QUANLYRAPCHIEUPHHIM.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface ITicketService
    {
        // Lấy danh sách vé
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        
        // Lấy thông tin chi tiết của một vé
        Task<Ticket> GetTicketByIdAsync(int id);
        
        // Lấy vé theo mã vé
        Task<Ticket> GetTicketByCodeAsync(string ticketCode);
        
        // Lấy danh sách vé theo trạng thái
        Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status);
        
        // Lấy danh sách vé theo người dùng
        Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId);
        
        // Lấy danh sách vé theo suất chiếu
        Task<IEnumerable<Ticket>> GetTicketsByShowtimeAsync(int showtimeId);
        
        // Lấy danh sách vé theo đặt vé
        Task<IEnumerable<Ticket>> GetTicketsByBookingAsync(int bookingId);
        
        // Tạo vé mới
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        
        // Cập nhật trạng thái vé
        Task<Ticket> UpdateTicketStatusAsync(int ticketId, string status);
        
        // Cập nhật thời gian quét vé
        Task<Ticket> UpdateScanDatetimeAsync(int ticketId, DateTime scanDatetime);
        
        // Hủy vé
        Task<bool> CancelTicketAsync(int ticketId);
        
        // Xác nhận vé
        Task<bool> ConfirmTicketAsync(int ticketId);
        
        // Kiểm tra xem vé có tồn tại không
        Task<bool> TicketExistsAsync(int id);
    }
} 