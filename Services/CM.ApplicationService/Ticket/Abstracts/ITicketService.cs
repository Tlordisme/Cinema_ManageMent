using CM.Domain.Ticket;
using CM.Dtos.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Ticket.Abstracts
{
    public interface ITicketService
    {
        Task<CMTicket> CreateTicket(int userId, string showtimeId, List<int> seatIds);
        Task<bool> PayTicket(int ticketId);
        Task<bool> CancelTicket(int ticketId);
    }
}
