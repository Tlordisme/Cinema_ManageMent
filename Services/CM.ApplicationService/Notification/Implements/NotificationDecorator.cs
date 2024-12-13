using CM.ApplicationService.Notification.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Notification.Implements
{
    public class NotificationDecorator : INotificationService
    {
        protected readonly INotificationService _wrappee;

        public NotificationDecorator(INotificationService wrappee)
        {
            _wrappee = wrappee;
        }

        public virtual async Task SendNotification(int ticketId)
        {
            await _wrappee.SendNotification(ticketId);
        }
    }

}
