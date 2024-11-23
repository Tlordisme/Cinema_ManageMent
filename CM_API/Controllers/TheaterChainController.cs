using CM.ApplicationService.Theater.Abstracts;
using CM.Dtos.Theater;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TheaterChainController : Controller
    {
        private readonly ITheaterChainService _theaterChainService;

        public TheaterChainController(ITheaterChainService theaterChainService)
        {
            _theaterChainService = theaterChainService;
        }

        [HttpPost]
        public IActionResult CreateTheaterChain([FromBody] TheaterChainDto dto)
        {
            var id = _theaterChainService.CreateTheaterChain(dto);
            return Ok(new { Message = "TheaterChain created successfully", TheaterChain = id });
        }

        [HttpGet]
        public IActionResult GetAllTheaterChains()
        {
            var theaterChains = _theaterChainService.GetAllTheaterChains();
            return Ok(theaterChains);
        }
    }
}
