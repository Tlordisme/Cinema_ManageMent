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

        public SeatController(ISeatService seatService, IPermissionService permissionService)
        {
            _seatService = seatService;
            _permissionService = permissionService;
        }

        [HttpGet("GetAllSeats{roomId}")]
        [Authorize]
        public IActionResult GetSeatsByRoomId(string roomId)
        {
            // Kiểm tra quyền lấy danh sách ghế theo phòng
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewSeatsByRoom))
            {
                return Unauthorized("Bạn không có quyền xem danh sách ghế theo phòng.");
            }

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

        [HttpPost("AddSeat")]
        [Authorize]
        public IActionResult AddSeat([FromBody] AddSeatDto seatDto)
        {
            // Kiểm tra quyền thêm ghế
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.AddSeat))
            {
                return Unauthorized("Bạn không có quyền thêm ghế.");
            }

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

        [HttpPut("UpdateSeat/{seatId}")]
        [Authorize]
        public IActionResult UpdateSeat([FromBody] UpdateSeatDto seatDto)
        {
            // Kiểm tra quyền cập nhật ghế
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateSeat))
            {
                return Unauthorized("Bạn không có quyền cập nhật ghế.");
            }

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

        [HttpPost("link-double-seat")]
        [Authorize]
        public IActionResult LinkDoubleSeat([FromBody] LinkDoubleSeatDto dto)
        {
            // Kiểm tra quyền liên kết ghế đôi
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.LinkDoubleSeat))
            {
                return Unauthorized("Bạn không có quyền liên kết ghế đôi.");
            }

            try
            {
                _seatService.LinkDoubleSeat(dto.SeatId, dto.DoubleSeatId);
                return Ok("Liên kết ghế đôi thành công.");
            }
            catch (Exception ex)
            {
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
                return Unauthorized("Bạn không có quyền xóa ghế.");
            }

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