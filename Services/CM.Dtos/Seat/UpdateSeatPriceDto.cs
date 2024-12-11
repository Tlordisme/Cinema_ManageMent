using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Seat
{
    public class UpdateSeatPriceDto : AddSeatPriceDto
    {
        public int Id { get; set; }
    }
}
