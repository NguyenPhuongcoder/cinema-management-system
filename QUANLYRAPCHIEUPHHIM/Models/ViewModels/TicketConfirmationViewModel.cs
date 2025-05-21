using System;
using System.Collections.Generic;

namespace QUANLYRAPCHIEUPHHIM.Models.ViewModels
{
    public class TicketConfirmationViewModel
    {
        public int TicketId { get; set; }
        public string TicketCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public decimal? DiscountValue { get; set; }

        // Customer Information
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }

        // Showtime Information
        public string MovieTitle { get; set; }
        public string FormatName { get; set; }
        public string CinemaName { get; set; }
        public string RoomName { get; set; }
        public DateTime Showtime { get; set; }

        // Seat Information
        public List<SeatInfo> Seats { get; set; }

        public class SeatInfo
        {
            public string SeatCode { get; set; }  // e.g., "A1", "B2"
            public string SeatType { get; set; }
            public decimal Price { get; set; }
        }
    }
} 