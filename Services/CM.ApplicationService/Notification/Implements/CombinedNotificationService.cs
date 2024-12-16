using CM.ApplicationService.Notification.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Notification.Implements
{
    public class CombinedNotificationService : INotificationService
    {
        private readonly INotificationService _emailService;
        private readonly INotificationService _smsService;

        public CombinedNotificationService(
            INotificationService emailService,
            INotificationService smsService)
        {
            _emailService = emailService;
            _smsService = smsService;
        }

        public async Task SendNotification(int ticketId)
        {
            // Gửi cả Email và SMS cùng lúc
            var emailTask = _emailService.SendNotification(ticketId);
            var smsTask = _smsService.SendNotification(ticketId);

            await Task.WhenAll(emailTask, smsTask);
        }
    }
}
