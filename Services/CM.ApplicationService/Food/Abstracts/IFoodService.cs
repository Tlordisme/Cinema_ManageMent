using CM.Dtos.Food;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Food.Abstracts
{
    public interface IFoodService
    {
        string CreateFood(FoodDto dto);
        FoodDto GetFoodById(string id);
        List<FoodDto> GetAllFoods();
        void UpdateFood(FoodDto dto);
        void DeleteFood(string id);
    }
}
