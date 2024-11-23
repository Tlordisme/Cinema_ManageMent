using CM.ApplicationService.Movie.Abstracts;
using CM.ApplicationService.Seat.Abstracts;
using CM.Domain.Seat;
using CM.Dtos.Movie;
using CM.Dtos.Seat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        // Lấy danh sách ghế theo phòng
        [HttpGet("{roomId}")]
        public IActionResult GetSeatsByRoomId(string roomId)
        {
            try
            {
                var seats = _seatService.GetSeatsByRoomId(roomId);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi lấy danh sách ghế: {ex.Message}");
            }
        }

        // Thêm ghế mới
        [HttpPost]
        public IActionResult AddSeat([FromBody] AddSeatDto seatDto)
        {
            try
            {
                _seatService.AddSeat(seatDto);
                return Ok("Thêm ghế thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi thêm ghế: {ex.Message}");
            }
        }

        // Cập nhật thông tin ghế
        [HttpPut]
        public IActionResult UpdateSeat([FromBody] UpdateSeatDto seatDto)
        {
            try
            {
                _seatService.UpdateSeat(seatDto);
                return Ok("Cập nhật ghế thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi cập nhật ghế: {ex.Message}");
            }
        }

        // Xóa ghế
        [HttpDelete("{seatId}")]
        public IActionResult DeleteSeat(int seatId)
        {
            try
            {
                _seatService.DeleteSeat(seatId);
                return Ok("Xóa ghế thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi xóa ghế: {ex.Message}");
            }
        }
    }
}
