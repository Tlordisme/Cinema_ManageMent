using CM.ApplicationService.Seat.Abstracts;
using CM.Dtos.Seat;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatPriceController : ControllerBase
    {
        private readonly ISeatPriceService _seatPriceService;

        public SeatPriceController(ISeatPriceService seatPriceService)
        {
            _seatPriceService = seatPriceService;
        }

        // Thêm giá ghế mới
        [HttpPost]
        public IActionResult AddSeatPrice([FromBody] AddSeatPriceDto seatPriceDto)
        {
            try
            {
                _seatPriceService.AddSeatPrice(seatPriceDto);
                return Ok("Thêm giá ghế thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi thêm giá ghế: {ex.Message}");
            }
        }

        // Cập nhật giá ghế
        [HttpPut]
        public IActionResult UpdateSeatPrice([FromBody] UpdateSeatPriceDto seatPriceDto)
        {
            try
            {
                _seatPriceService.UpdateSeatPrice(seatPriceDto);
                return Ok("Cập nhật giá ghế thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi cập nhật giá ghế: {ex.Message}");
            }
        }

        // Xóa giá ghế
        [HttpDelete("{seatPriceId}")]
        public IActionResult DeleteSeatPrice(int seatPriceId)
        {
            try
            {
                _seatPriceService.DeleteSeatPrice(seatPriceId);
                return Ok("Xóa giá ghế thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi xóa giá ghế: {ex.Message}");
            }
        }

        // Lấy danh sách giá ghế theo phòng
        [HttpGet("{roomId}")]
        public IActionResult GetSeatPricesByRoomId(string roomId)
        {
            try
            {
                var seatPrices = _seatPriceService.GetSeatPricesByRoomId(roomId);
                return Ok(seatPrices);
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi lấy giá ghế: {ex.Message}");
            }
        }

        // Lấy giá ghế theo ID
        [HttpGet("{seatPriceId}")]
        public IActionResult GetSeatPrice(int seatPriceId)
        {
            try
            {
                var seatPrice = _seatPriceService.GetSeatPrice(seatPriceId);
                return Ok(seatPrice);
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi lấy giá ghế: {ex.Message}");
            }
        }
    }
}
