using QUANLYRAPCHIEUPHHIM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QUANLYRAPCHIEUPHHIM.Services
{
    public interface ITicketService
    {
        IEnumerable<Ticket> GetAllTickets();
        Ticket GetTicketById(int id);
        Task<Ticket> GetTicketByIdAsync(int id);
        void CreateTicket(Ticket ticket);
        void UpdateTicket(Ticket ticket);
        Task UpdateTicketAsync(Ticket ticket);
        void DeleteTicket(int id);
        IEnumerable<int> GetBookedSeats(int showtimeId);
        Task<IEnumerable<Ticket>> GetTicketsAsync(
            string ticketCode = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10
        );
        Task<int> CountTicketsAsync(
            string ticketCode = null,
            string customerPhone = null,
            int? movieId = null,
            string status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null
        );
    }
} 