using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Revenue.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;
using System.Linq;
using System.Threading.Tasks;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _revenueService;
        private readonly IPermissionService _permissionService;

        public RevenueController(IRevenueService revenueService, IPermissionService permissionService)
        {
            _revenueService = revenueService;
            _permissionService = permissionService;
        }

        // Kiểm tra quyền và thống kê doanh thu theo ngày
        [HttpGet("revenue-by-date/{date}")]
        [Authorize]
        public async Task<IActionResult> GetRevenueByDate([FromQuery] string date)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền
            if (!_permissionService.CheckPermission(userId, PermissionKey.ViewRevenue))
            {
                return Unauthorized("You do not have permission to view revenue.");
            }

            try
            {
                var revenueData = await _revenueService.GetRevenueByDateAsync(date);
                return Ok(revenueData);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving revenue by date.", Error = ex.Message });
            }
        }

        // Kiểm tra quyền và thống kê doanh thu theo bộ phim
        [HttpGet("revenue-by-movie/{MovieId}")]
        [Authorize]
        public async Task<IActionResult> GetRevenueByMovie([FromQuery] int? movieId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền
            if (!_permissionService.CheckPermission(userId, PermissionKey.ViewRevenue))
            {
                return Unauthorized("You do not have permission to view revenue.");
            }

            try
            {
                var revenueData = await _revenueService.GetRevenueByMovieAsync(movieId);
                return Ok(revenueData);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving revenue by movie.", Error = ex.Message });
            }
        }
    }
}
