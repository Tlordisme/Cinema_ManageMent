using CM.ApplicationService.Showtime.Abstracts;
using CM.ApplicationService.Showtime.Implements;
using CM.Dtos.Showtime;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimeController : Controller
    {
        private readonly IShowtimeService _showtimeService;

        public ShowtimeController(IShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var showtimes = await _showtimeService.GetAllShowtimesAsync();
            return Ok(showtimes);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var showtime = await _showtimeService.GetShowtimeByIdAsync(id);
                return Ok(showtime);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShowtimeDto createShowtimeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newShowtime = await _showtimeService.CreateShowtimeAsync(createShowtimeDto);
                return CreatedAtAction(nameof(GetById), new { id = newShowtime.Id }, newShowtime);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateShowtimeDto updateShowtimeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updateShowtimeDto.Id)
                return BadRequest(new { message = "ID mismatch" });

            try
            {
                var updated = await _showtimeService.UpdateShowtimeAsync(updateShowtimeDto);
                if (updated)
                    return NoContent();

                return NotFound(new { message = "Showtime not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _showtimeService.DeleteShowtimeAsync(id);
                if (deleted)
                    return NoContent();

                return NotFound(new { message = "Showtime not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
