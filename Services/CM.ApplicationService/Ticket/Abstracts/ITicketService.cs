using CM.Domain.Ticket;
using CM.Dtos.Ticket;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Ticket.Abstracts
{
    public interface ITicketService
    {
         Task<CMTicket> BookTicketAsync(int userId, CreateTicketDto request, HttpContext context);
         Task<TicketDetailsDto> GetTicketDetailsAsync(int ticketId);

         Task CancelTicketIfNotPaid(int ticketId);
        //Task<string> BookTicketAndCreatePaymentUrlAsync(int userId, string showtimeId, List<int> seatIds, HttpContext context);


    }
}
