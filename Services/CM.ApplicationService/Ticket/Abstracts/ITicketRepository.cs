using CM.Dtos.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Ticket.Abstracts
{
    public interface ITicketRepository
    {
        Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId);
        Task<bool> DeleteTicket(int ticketId);
        Task<List<TicketDetailsDto>> GetTicketsByUserId(int userId);
        Task<List<TicketDetailsDto>> GetAllTickets();
    }
}