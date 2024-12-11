using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Notification.Abstracts
{
    public interface IEmailLogicService
    {
        Task<string> GenerateEmailBodyAsync(int ticketId);
    }
}
