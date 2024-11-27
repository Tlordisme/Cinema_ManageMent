using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Common;
using CM.ApplicationService.Food.Abstracts;
using fooddomain = CM.Domain.Food;
using CM.Dtos.Food;
using CM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Share.ApplicationService;

namespace CM.ApplicationService.Food.Implements
{
    public class FoodService : ServiceBase, IFoodService
    {
        public FoodService(CMDbContext dbContext, ILogger<FoodService> logger)
            : base(logger, dbContext) { }

        // Create Food
        public string CreateFood(FoodDto dto)
        {
            var food = new fooddomain.Food
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description
            };

            _dbContext.Foods.Add(food);
            _dbContext.SaveChanges();

            return food.Id;
        }

        // Get Food by Id
        public FoodDto GetFoodById(string id)
        {
            var food = _dbContext.Foods.FirstOrDefault(f => f.Id == id);
            if (food == null)
                throw new Exception("Food not found.");

            return new FoodDto
            {
                Id = food.Id,
                Name = food.Name,
                Price = food.Price,
                Description = food.Description
            };
        }

        // Get all Foods
        public List<FoodDto> GetAllFoods()
        {
            return _dbContext.Foods
                .Select(f => new FoodDto
                {
                    Id = f.Id,
                    Name = f.Name,
                    Price = f.Price,
                    Description = f.Description
                })
                .ToList();
        }

        // Update Food
        public void UpdateFood(FoodDto dto)
        {
            var food = _dbContext.Foods.Find(dto.Id);
            if (food == null)
                throw new Exception("Food not found.");

            food.Name = dto.Name;
            food.Price = dto.Price;
            food.Description = dto.Description;

            _dbContext.SaveChanges();
        }

        // Delete Food
        public void DeleteFood(string id)
        {
            var food = _dbContext.Foods.Find(id);
            if (food == null)
                throw new Exception("Food not found.");

            _dbContext.Foods.Remove(food);
            _dbContext.SaveChanges();
        }
    }


}
