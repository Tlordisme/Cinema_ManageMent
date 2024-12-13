using CM.ApplicationService.Notification.Abstracts;
using CM.ApplicationService.Ticket.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Notification.Implements
{
    public class SmsService : INotificationService
    {
        private readonly ITicketRepository _ticketRepository;

        public SmsService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task SendNotification(int ticketId)
        {


            // Giả lập gửi SMS
            Console.WriteLine($"Sending SMS to  Booking Successful!");
            await Task.CompletedTask;
        }
    }
    }
