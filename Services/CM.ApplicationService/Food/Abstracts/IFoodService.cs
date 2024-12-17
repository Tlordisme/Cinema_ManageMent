using CM.Dtos.Food;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CM.ApplicationService.Food.Abstracts
{
    public interface IFoodService
    {
        Task<List<FoodDto>> GetAllFoodsAsync();
        Task<FoodDto> GetFoodByIdAsync(int id);
        Task<FoodDto> AddFoodAsync(FoodDto foodDto);
        Task<FoodDto> UpdateFoodAsync(FoodDto foodDto);
        Task<bool> DeleteFoodAsync(int id);
    }
}
