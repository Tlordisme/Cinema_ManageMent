using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Food
{
    public class FoodItemDto
    {
        public string FoodId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}