using CM.ApplicationService.Food.Abstracts;
using CM.Dtos.Food;
using CM.Domain.Food;
using CM.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using CM.ApplicationService.Common;
using Microsoft.EntityFrameworkCore;

namespace CM.ApplicationService.Food.Implements
{
    public class ComboService : ServiceBase, IComboService
    {
        public ComboService(CMDbContext dbContext, ILogger<ComboService> logger)
            : base(logger, dbContext) { }

        // Thêm Combo mới
        public string CreateCombo(AddOrUpdateComboDto comboDto)
        {
            var combo = new Combo
            {
                Id = Guid.NewGuid().ToString(),
                Name = comboDto.Name,
                Price = comboDto.Price,
                Foods = _dbContext.Foods.Where(f => comboDto.FoodIds.Contains(f.Id)).ToList() // Lấy các món ăn từ bảng Foods
            };

            _dbContext.Combos.Add(combo);
            _dbContext.SaveChanges();

            return combo.Id;
        }

        // Lấy Combo theo ID
        public ComboDto GetComboById(string comboId)
        {
            var combo = _dbContext.Combos
                .Where(c => c.Id == comboId)
                .Select(c => new ComboDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    Foods = c.Foods.Select(f => new FoodDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Price = f.Price,
                        Description = f.Description
                    }).ToList()
                })
                .FirstOrDefault();

            if (combo == null)
                throw new Exception("Combo không tồn tại.");

            return combo;
        }

        // Lấy tất cả Combos
        public List<ComboDto> GetAllCombos()
        {
            return _dbContext.Combos
                .Select(c => new ComboDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    Foods = c.Foods.Select(f => new FoodDto
                    {
                        Id = f.Id,
                        Name = f.Name,
                        Price = f.Price,
                        Description = f.Description
                    }).ToList()
                })
                .ToList();
        }

        // Cập nhật Combo
        public void UpdateCombo(ComboDto comboDto)
        {
            var combo = _dbContext.Combos.Include(c => c.Foods).FirstOrDefault(c => c.Id == comboDto.Id);
            if (combo == null)
                throw new Exception("Combo không tồn tại.");

            combo.Name = comboDto.Name;
            combo.Price = comboDto.Price;
            combo.Foods = _dbContext.Foods.Where(f => comboDto.Foods.Select(fd => fd.Id).Contains(f.Id)).ToList();

            _dbContext.SaveChanges();
        }

        // Xóa Combo
        public void DeleteCombo(string comboId)
        {
            var combo = _dbContext.Combos.Find(comboId);
            if (combo == null)
                throw new Exception("Combo không tồn tại.");

            _dbContext.Combos.Remove(combo);
            _dbContext.SaveChanges();
        }
    }
}
