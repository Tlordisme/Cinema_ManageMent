//using CM.ApplicationService.Movie.Abstracts;
//using CM.ApplicationService.Seat.Abstracts;
//using CM.Dtos.Movie;
//using CM.Dtos.Seat;
//using Microsoft.AspNetCore.Mvc;

//namespace CM_API.Controllers
//{
//    public class SeatController : Controller
//    {
//        private readonly ISeatService _seatService;

//        public SeatController(ISeatService seatService)
//        {
//            _seatService = seatService;
//        }

//        // Endpoint để book một chỗ ngồi
//        [HttpPost("book/{seatId}")]
//        public async Task<ActionResult<SeatResponseDto>> BookSeat(int seatId)
//        {
//            var seat = await _seatService.BookSeat(seatId);
//            if (seat == null)
//            {
//                return NotFound($"Seat with ID {seatId} is not available.");
//            }
//            return Ok(seat);
//        }

//        // Endpoint để lấy tất cả các chỗ ngồi theo RoomID
//        [HttpGet("room/{roomId}")]
//        public async Task<ActionResult<IEnumerable<SeatResponseDto>>> GetSeatsByRoomId(string roomId)
//        {
//            var seats = await _seatService.GetSeatsByRoomId(roomId);
//            if (seats == null)
//            {
//                return NotFound($"No seats found for room with ID {roomId}.");
//            }
//            return Ok(seats);
//        }

//        // Endpoint để giải phóng một chỗ ngồi
//        [HttpPost("release/{seatId}")]
//        public async Task<ActionResult> ReleaseSeat(int seatId)
//        {
//            var result = await _seatService.ReleaseSeat(seatId);
//            if (!result)
//            {
//                return BadRequest($"Seat with ID {seatId} is already available or does not exist.");
//            }
//            return NoContent(); // No Content: Request was successful but there's no data to return
//        }
//    }
//}
