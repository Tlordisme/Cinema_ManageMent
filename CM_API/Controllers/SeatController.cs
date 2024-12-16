using CM.ApplicationService.Movie.Abstracts;
using CM.ApplicationService.Seat.Abstracts;
using CM.Domain.Seat;
using CM.Dtos.Movie;
using CM.Dtos.Seat;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CM.ApplicantService.Auth.Permission.Abstracts;
using Share.Constant.Permission;
using Microsoft.AspNetCore.Authorization;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<SeatController> _logger;

        public SeatController(ISeatService seatService, IPermissionService permissionService, ILogger<SeatController> logger)
        {
            _seatService = seatService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpGet("GetAllSeats{roomId}")]
        [Authorize]
        public IActionResult GetSeatsByRoomId(string roomId)
        {
            // Kiểm tra quyền lấy danh sách ghế theo phòng
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewSeatsByRoom))
            {
                _logger.LogWarning("User with ID {UserId} attempted to view seats without the required permission.", currentUserId);
                return Unauthorized("Bạn không có quyền xem danh sách ghế theo phòng.");
            }

            try
            {
                var seats = _seatService.GetSeatsByRoomId(roomId);
                _logger.LogInformation("User with ID {UserId} successfully fetched seats for room {RoomId}.", currentUserId, roomId);
                return Ok(seats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching seats for room {RoomId}.", roomId);
                return BadRequest($"Có lỗi khi lấy danh sách ghế: {ex.Message}");
            }
        }

        [HttpPost("AddSeat")]
        [Authorize]
        public IActionResult AddSeat([FromBody] AddSeatDto seatDto)
        {
            // Kiểm tra quyền thêm ghế
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.AddSeat))
            {
                _logger.LogWarning("User with ID {UserId} attempted to add a seat without the required permission.", currentUserId);
                return Unauthorized("Bạn không có quyền thêm ghế.");
            }

            try
            {
                _seatService.AddSeat(seatDto);
                _logger.LogInformation("User with ID {UserId} successfully added a new seat.", currentUserId);
                return Ok("Thêm ghế thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new seat.");
                return BadRequest($"Có lỗi khi thêm ghế: {ex.Message}");
            }
        }

        [HttpPut("UpdateSeat/{seatId}")]
        [Authorize]
        public IActionResult UpdateSeat([FromBody] UpdateSeatDto seatDto)
        {
            // Kiểm tra quyền cập nhật ghế
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateSeat))
            {
                _logger.LogWarning("User with ID {UserId} attempted to update seat without the required permission.", currentUserId);
                return Unauthorized("Bạn không có quyền cập nhật ghế.");
            }

            try
            {
                _seatService.UpdateSeat(seatDto);
                _logger.LogInformation("User with ID {UserId} successfully updated seat.", currentUserId);
                return Ok("Cập nhật ghế thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating seat.");
                return BadRequest($"Có lỗi khi cập nhật ghế: {ex.Message}");
            }
        }

        [HttpPost("link-double-seat")]
        [Authorize]
        public IActionResult LinkDoubleSeat([FromBody] LinkDoubleSeatDto dto)
        {
            // Kiểm tra quyền liên kết ghế đôi
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.LinkDoubleSeat))
            {
                _logger.LogWarning("User with ID {UserId} attempted to link double seats without the required permission.", currentUserId);
                return Unauthorized("Bạn không có quyền liên kết ghế đôi.");
            }

            try
            {
                _seatService.LinkDoubleSeat(dto.SeatId, dto.DoubleSeatId);
                _logger.LogInformation("User with ID {UserId} successfully linked double seats: Seat ID {SeatId} with Double Seat ID {DoubleSeatId}.", currentUserId, dto.SeatId, dto.DoubleSeatId);
                return Ok("Liên kết ghế đôi thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while linking double seats: Seat ID {SeatId} and Double Seat ID {DoubleSeatId}.", dto.SeatId, dto.DoubleSeatId);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteSeat/{seatId}")]
        [Authorize]
        public IActionResult DeleteSeat(int seatId)
        {
            // Kiểm tra quyền xóa ghế
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteSeat))
            {
                _logger.LogWarning("User with ID {UserId} attempted to delete seat with ID {SeatId} without the required permission.", currentUserId, seatId);
                return Unauthorized("Bạn không có quyền xóa ghế.");
            }

            try
            {
                _seatService.DeleteSeat(seatId);
                _logger.LogInformation("User with ID {UserId} successfully deleted seat with ID {SeatId}.", currentUserId, seatId);
                return Ok("Xóa ghế thành công!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting seat with ID {SeatId}.", seatId);
                return BadRequest($"Có lỗi khi xóa ghế: {ex.Message}");
            }
        }
    }
}