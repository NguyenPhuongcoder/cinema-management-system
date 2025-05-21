using System;

namespace QUANLYRAPCHIEUPHHIM.Models.ViewModels
{
    public class TicketSearchViewModel
    {
        public string TicketCode { get; set; }
        public string CustomerPhone { get; set; }
        public int? MovieId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
        public int StartRecord => (Page - 1) * PageSize + 1;
        public int EndRecord => Math.Min(Page * PageSize, TotalRecords);
        public IEnumerable<Ticket> Tickets { get; set; }
    }
} 