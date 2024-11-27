using CM.Domain.Food;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.Domain.Food
{
    public class Combo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public List<Food> Foods { get; set; } = new List<Food>();
    }
}
