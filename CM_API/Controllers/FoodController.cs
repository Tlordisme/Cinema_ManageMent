using CM.ApplicationService.Food.Abstracts;
using CM.Dtos.Food;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        // POST: api/food
        [HttpPost]
        public ActionResult<string> CreateFood([FromBody] FoodDto foodDto)
        {
            try
            {
                var foodId = _foodService.CreateFood(foodDto);
                return Ok(new { Id = foodId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/food/{id}
        [HttpGet("{id}")]
        public ActionResult<FoodDto> GetFoodById(string id)
        {
            try
            {
                var food = _foodService.GetFoodById(id);
                return Ok(food);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/food
        [HttpGet]
        public ActionResult<List<FoodDto>> GetAllFoods()
        {
            try
            {
                var foods = _foodService.GetAllFoods();
                return Ok(foods);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/food/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateFood(string id, [FromBody] FoodDto foodDto)
        {
            try
            {
                foodDto.Id = id;  // Ensure that the DTO ID matches the parameter
                _foodService.UpdateFood(foodDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/food/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteFood(string id)
        {
            try
            {
                _foodService.DeleteFood(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
