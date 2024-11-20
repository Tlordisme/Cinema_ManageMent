using CM.ApplicationService.Theater.Abstracts;
using CM.Dtos.Theater;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheaterController : ControllerBase
    {
        private readonly ITheaterService _theaterService;

        public TheaterController(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        [HttpPost]
        public IActionResult CreateTheater([FromBody] TheaterDto dto)
        {
            try
            {
                var id = _theaterService.CreateTheater(dto);
                return CreatedAtAction(nameof(GetTheatersByChainId), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{chainId}")]
        public IActionResult GetTheatersByChainId(string chainId)
        {
            var theaters = _theaterService.GetTheatersByChainId(chainId);
            return Ok(theaters);
        }
    }
}
