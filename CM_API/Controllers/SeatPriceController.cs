using CM.ApplicationService.Seat.Abstracts;
using CM.Dtos.Seat;
using CM.ApplicantService.Auth.Permission.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatPriceController : ControllerBase
    {
        private readonly ISeatPriceService _seatPriceService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<SeatPriceController> _logger;

        public SeatPriceController(ISeatPriceService seatPriceService, IPermissionService permissionService, ILogger<SeatPriceController> logger)
        {
            _seatPriceService = seatPriceService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("AddSeatPrice")]
        [Authorize]
        public IActionResult AddSeatPrice([FromBody] AddSeatPriceDto seatPriceDto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            _logger.LogInformation($"User {currentUserId} is trying to add a new seat price.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.AddSeatPrice))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to add seat price.");
                return Unauthorized("Bạn không có quyền thêm giá ghế.");
            }

            try
            {
                _seatPriceService.AddSeatPrice(seatPriceDto);
                _logger.LogInformation("Seat price added successfully.");
                return Ok("Thêm giá ghế thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding seat price.");
                return BadRequest($"Có lỗi khi thêm giá ghế: {ex.Message}");
            }
        }

        [HttpPut("UpdateSeatPrice")]
        [Authorize]
        public IActionResult UpdateSeatPrice([FromBody] UpdateSeatPriceDto seatPriceDto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            _logger.LogInformation($"User {currentUserId} is trying to update a seat price.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateSeatPrice))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to update seat price.");
                return Unauthorized("Bạn không có quyền cập nhật giá ghế.");
            }

            try
            {
                _seatPriceService.UpdateSeatPrice(seatPriceDto);
                _logger.LogInformation("Seat price updated successfully.");
                return Ok("Cập nhật giá ghế thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating seat price.");
                return BadRequest($"Có lỗi khi cập nhật giá ghế: {ex.Message}");
            }
        }

        [HttpDelete("DeleteSeatPrice/{seatPriceId}")]
        [Authorize]
        public IActionResult DeleteSeatPrice(int seatPriceId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            _logger.LogInformation($"User {currentUserId} is trying to delete seat price with ID {seatPriceId}.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteSeatPrice))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to delete seat price.");
                return Unauthorized("Bạn không có quyền xóa giá ghế.");
            }

            try
            {
                _seatPriceService.DeleteSeatPrice(seatPriceId);
                _logger.LogInformation($"Seat price with ID {seatPriceId} deleted successfully.");
                return Ok("Xóa giá ghế thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting seat price.");
                return BadRequest($"Có lỗi khi xóa giá ghế: {ex.Message}");
            }
        }

        [HttpGet("GetPriceOfTypeSeat/{roomId}")]
        [Authorize]
        public IActionResult GetSeatPricesByRoomId(string roomId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            _logger.LogInformation($"User {currentUserId} is trying to get seat prices for room ID {roomId}.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewSeatPricesByRoom))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to view seat prices by room.");
                return Unauthorized("Bạn không có quyền xem danh sách giá ghế theo phòng.");
            }

            try
            {
                var seatPrices = _seatPriceService.GetSeatPricesByRoomId(roomId);
                _logger.LogInformation($"Successfully retrieved seat prices for room ID {roomId}.");
                return Ok(seatPrices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving seat prices.");
                return BadRequest($"Có lỗi khi lấy giá ghế: {ex.Message}");
            }
        }

        [HttpGet("GetPriceOfType/{seatPriceId}")]
        [Authorize]
        public IActionResult GetSeatPrice(int seatPriceId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

            _logger.LogInformation($"User {currentUserId} is trying to get seat price with ID {seatPriceId}.");

            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewSeatPriceById))
            {
                _logger.LogWarning($"User {currentUserId} does not have permission to view seat price by ID.");
                return Unauthorized("Bạn không có quyền xem giá ghế theo ID.");
            }

            try
            {
                var seatPrice = _seatPriceService.GetSeatPrice(seatPriceId);
                _logger.LogInformation($"Successfully retrieved seat price with ID {seatPriceId}.");
                return Ok(seatPrice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving seat price.");
                return BadRequest($"Có lỗi khi lấy giá ghế: {ex.Message}");
            }
        }
    }
}