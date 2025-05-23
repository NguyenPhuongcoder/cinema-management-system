using System;
using System.Collections.Generic;
using QUANLYRAPCHIEUPHHIM.Models;

namespace QUANLYRAPCHIEUPHHIM.ViewModels
{
    public class BookingPaymentViewModel
    {
        public Movie Movie { get; set; }
        public Cinema Cinema { get; set; }
        public Showtime Showtime { get; set; }
        public Room Room { get; set; }
        public List<Seat> SelectedSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string> PaymentMethods { get; set; }
        public string SelectedPaymentMethod { get; set; }
        public string PromoCode { get; set; }
    }
} 