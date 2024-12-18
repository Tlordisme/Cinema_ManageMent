using CM.ApplicationService.Food.Abstracts;
using CM.ApplicationService.Theater.Abstracts;
using CM.Domain.Food;
using CM.Dtos.Food;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CM.ApplicationService.Food
{
    public class FoodService : IFoodService
    {
        private readonly CMDbContext _dbContext;
        private readonly ILogger<FoodService> _logger;

        public FoodService(CMDbContext dbContext, ILogger<FoodService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // Lấy tất cả thực phẩm và chuyển sang FoodDto
        public async Task<List<FoodDto>> GetAllFoodsAsync()
        {
            var foods = await _dbContext.Foods.ToListAsync();
            _logger.LogInformation("Retrieved all foods from database.");

            return foods.Select(f => new FoodDto
            {
                Id = f.Id.ToString(),  // Chuyển đổi kiểu dữ liệu nếu cần
                Name = f.Name,
                Description = f.Description,
                Price = f.Price
            }).ToList();
        }

        // Lấy thông tin chi tiết thực phẩm theo Id và trả về FoodDto
        public async Task<FoodDto> GetFoodByIdAsync(int id)
        {
            var food = await _dbContext.Foods.FindAsync(id);
            if (food == null)
            {
                _logger.LogWarning($"Food with ID {id} not found.");
                return null;
            }

            _logger.LogInformation($"Retrieved food with ID {id}.");
            return new FoodDto
            {
                Id = food.Id.ToString(),
                Name = food.Name,
                Description = food.Description,
                Price = food.Price
            };
        }

        // Thêm thực phẩm mới và trả về FoodDto
        public async Task<FoodDto> AddFoodAsync(FoodDto foodDto)
        {
            var food = new FoFood
            {
                Name = foodDto.Name,
                Description = foodDto.Description,
                Price = foodDto.Price
            };

            _dbContext.Foods.Add(food);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Added new food with ID {food.Id}.");

            return new FoodDto
            {
                Id = food.Id.ToString(),
                Name = food.Name,
                Description = food.Description,
                Price = food.Price
            };
        }

        // Cập nhật thực phẩm và trả về FoodDto
        public async Task<FoodDto> UpdateFoodAsync(FoodDto foodDto)
        {
            var food = await _dbContext.Foods.FindAsync(int.Parse(foodDto.Id));
            if (food == null)
            {
                _logger.LogWarning($"Food with ID {foodDto.Id} not found.");
                return null;
            }

            food.Name = foodDto.Name;
            food.Description = foodDto.Description;
            food.Price = foodDto.Price;

            _dbContext.Foods.Update(food);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Updated food with ID {food.Id}.");

            return new FoodDto
            {
                Id = food.Id.ToString(),
                Name = food.Name,
                Description = food.Description,
                Price = food.Price
            };
        }

        // Xóa thực phẩm và trả về kết quả
        public async Task<bool> DeleteFoodAsync(int id)
        {
            var food = await _dbContext.Foods.FindAsync(id);
            if (food == null)
            {
                _logger.LogWarning($"Food with ID {id} not found.");
                return false;
            }

            _dbContext.Foods.Remove(food);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Deleted food with ID {id}.");

            return true;
        }
    }
}
