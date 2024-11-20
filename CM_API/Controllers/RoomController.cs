using CM.ApplicationService.Theater.Abstracts;
using CM.Dtos.Theater;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        public IActionResult CreateRoom([FromBody] RoomDto dto)
        {
            try
            {
                var id = _roomService.CreateRoom(dto);
                return CreatedAtAction(nameof(GetRoomsByTheaterId), new { id }, new { id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{theaterId}")]
        public IActionResult GetRoomsByTheaterId(string theaterId)
        {
            var rooms = _roomService.GetRoomsByTheaterId(theaterId);
            return Ok(rooms);
        }
    }
}
