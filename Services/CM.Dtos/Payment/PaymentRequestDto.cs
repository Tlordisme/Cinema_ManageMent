using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Payment
{
    public class PaymentRequestDto
    {
        public int TicketId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string ReturnUrl { get; set; }
    }
}
