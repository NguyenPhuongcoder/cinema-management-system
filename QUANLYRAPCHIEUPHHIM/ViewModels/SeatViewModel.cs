using System;
using System.Collections.Generic;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class SeatViewModel
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public List<Seat> Seats { get; set; }
        public List<SeatType> AvailableSeatTypes { get; set; }
    }
} 