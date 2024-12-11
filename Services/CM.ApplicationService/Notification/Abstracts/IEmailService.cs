using CM.Dtos.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Notification.Abstracts
{
    public interface IEmailService
    {
        public Task SendEmailAsync(int ticketId); // Thay đổi từ async void thành async Task

    }
}
