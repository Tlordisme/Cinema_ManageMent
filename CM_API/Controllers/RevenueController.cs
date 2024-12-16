using CM.ApplicationService.Revenue.Abstracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _revenueService;

        public RevenueController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

        // Thống kê doanh thu theo ngày
        [HttpGet("revenue-by-date")]
        public async Task<IActionResult> GetRevenueByDate()
        {
            try
            {
                var revenueData = await _revenueService.GetRevenueByDateAsync();
                return Ok(revenueData); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving revenue by date.", Error = ex.Message });
            }
        }

        // Thống kê doanh thu theo bộ phim
        [HttpGet("revenue-by-movie")]
        public async Task<IActionResult> GetRevenueByMovie()
        {
            try
            {
                var movieRevenueData = await _revenueService.GetRevenueByMovieAsync();
                return Ok(movieRevenueData); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while retrieving revenue by movie.", Error = ex.Message });
            }
        }
    }
}
