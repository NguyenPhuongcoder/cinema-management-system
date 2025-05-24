using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface IBookingService
    {
        
        // Tạo đặt vé mới
        Task<Booking> CreateBookingAsync(Booking booking);
        
        // Cập nhật trạng thái đặt vé
        Task<Booking> UpdateBookingStatusAsync(int bookingId, string status);

        // Cập nhật thông tin đặt vé
        Task<Booking> UpdateBookingAsync(Booking booking);
        
        // Hủy đặt vé
        Task<bool> CancelBookingAsync(int bookingId);
    }
} 