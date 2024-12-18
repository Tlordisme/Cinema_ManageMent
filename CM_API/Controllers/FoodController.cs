using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Food.Abstracts;
using CM.Dtos.Food;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;
using System.Linq;
using System.Threading.Tasks;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly IPermissionService _permissionService;

        public FoodController(IFoodService foodService, IPermissionService permissionService)
        {
            _foodService = foodService;
            _permissionService = permissionService;
        }

        // Kiểm tra quyền và lấy tất cả món ăn
        [HttpGet("GetAllFoods")]
        [Authorize]
        public async Task<IActionResult> GetAllFoods()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền
            if (!_permissionService.CheckPermission(userId, PermissionKey.GetAllFoods))
            {
                return Unauthorized("You do not have permission to view foods.");
            }

            var foods = await _foodService.GetAllFoodsAsync();
            return Ok(foods);
        }

        // Kiểm tra quyền và lấy thông tin món ăn theo ID
        [HttpGet("GetFood/{id}")]
        [Authorize]
        public async Task<IActionResult> GetFoodById(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền
            if (!_permissionService.CheckPermission(userId, PermissionKey.ViewFood))
            {
                return Unauthorized("You do not have permission to view this food.");
            }

            var food = await _foodService.GetFoodByIdAsync(id);
            if (food == null)
                return NotFound(new { Message = "Food not found" });

            return Ok(food);
        }

        // Kiểm tra quyền và thêm món ăn mới
        [HttpPost("AddFood")]
        [Authorize]
        public async Task<IActionResult> AddFood([FromBody] FoodDto foodDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền
            if (!_permissionService.CheckPermission(userId, PermissionKey.AddFood))
            {
                return Unauthorized("You do not have permission to add food.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedFood = await _foodService.AddFoodAsync(foodDto);
            return CreatedAtAction(nameof(GetFoodById), new { id = addedFood.Id }, addedFood);
        }

        // Kiểm tra quyền và cập nhật món ăn
        [HttpPut("UpdateFood/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFood(int id, [FromBody] FoodDto foodDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền
            if (!_permissionService.CheckPermission(userId, PermissionKey.UpdateFood))
            {
                return Unauthorized("You do not have permission to edit food.");
            }

            if (id != int.Parse(foodDto.Id))
                return BadRequest("Food ID mismatch");

            var updatedFood = await _foodService.UpdateFoodAsync(foodDto);
            if (updatedFood == null)
                return NotFound(new { Message = "Food not found" });

            return Ok(updatedFood);
        }

        // Kiểm tra quyền và xóa món ăn
        [HttpDelete("DeleteFood/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFood(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền
            if (!_permissionService.CheckPermission(userId, PermissionKey.DeleteFood))
            {
                return Unauthorized("You do not have permission to delete food.");
            }

            var isDeleted = await _foodService.DeleteFoodAsync(id);
            if (!isDeleted)
                return NotFound(new { Message = "Food not found" });

            return NoContent();
        }
    }
}