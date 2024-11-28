using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Payment
{
    public class VnPayRequest
    {
        public string FullName { get; set; }
        public string Description { get; set; }

        public int TicketId { get; set; }
        public decimal  Amount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
