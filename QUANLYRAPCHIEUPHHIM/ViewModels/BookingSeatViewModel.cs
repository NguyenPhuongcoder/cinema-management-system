using System;
using System.Collections.Generic;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class BookingSeatViewModel
    {
        public Movie Movie { get; set; }
        public Cinema Cinema { get; set; }
        public Showtime Showtime { get; set; }
        public Room Room { get; set; }
        public List<SeatStatusViewModel> Seats { get; set; }
        public List<int> SelectedSeatIds { get; set; }
    }
    public class SeatStatusViewModel
    {
        public int SeatId { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public bool IsSold { get; set; }
        public string Type { get; set; } // VIP, Đơn, Đôi, Ba
        public decimal Price { get; set; }
    }
} 