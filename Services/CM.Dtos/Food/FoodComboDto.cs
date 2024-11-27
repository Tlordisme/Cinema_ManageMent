using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Dtos.Food
{
    public class AddOrUpdateComboDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<string> FoodIds { get; set; } 
    }

    public class ComboDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<FoodDto> Foods { get; set; } 
    }
}