using CM.Dtos.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Notification.Abstracts
{
    public interface IEmailTemplateService
    {
        Task<string> GenerateEmailContent(TicketDetailsDto ticketDetailsDto);
    }
}
