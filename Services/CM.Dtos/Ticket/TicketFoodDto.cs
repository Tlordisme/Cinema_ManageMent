using CM.Dtos.Food;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Ticket
{
    public class TicketFoodDto
    {
        public List<FoodItemDto> FoodItems { get; set; }
        public int TicketId { get; set; }
    }
}
