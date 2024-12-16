using CM.ApplicationService.Theater.Abstracts;
using CM.Dtos.Theater;
using Microsoft.AspNetCore.Mvc;
using CM.ApplicantService.Auth.Permission.Abstracts;
using Share.Constant.Permission;
using Microsoft.AspNetCore.Authorization;

namespace CM_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, IPermissionService permissionService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _permissionService = permissionService;
            _logger = logger;
        }

        [HttpPost("AddRoom")]
        [Authorize]
        public IActionResult CreateRoom([FromBody] RoomDto dto)
        {
            // Kiểm tra quyền thêm phòng
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.AddRoom))
            {
                _logger.LogWarning($"User {currentUserId} tried to add a room without permission.");
                return Unauthorized("Bạn không có quyền thêm phòng.");
            }

            try
            {
                var id = _roomService.CreateRoom(dto);
                _logger.LogInformation($"Room created successfully with ID {id} by user {currentUserId}.");
                return CreatedAtAction(nameof(GetRoomsByTheaterId), new { id }, new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while creating room: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllRooms/{theaterId}")]
        [Authorize]
        public IActionResult GetRoomsByTheaterId(string theaterId)
        {
            // Kiểm tra quyền lấy danh sách phòng
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewRoomsByTheater))
            {
                _logger.LogWarning($"User {currentUserId} tried to view rooms without permission for theater {theaterId}.");
                return Unauthorized("Bạn không có quyền xem danh sách phòng của rạp.");
            }

            try
            {
                var rooms = _roomService.GetRoomsByTheaterId(theaterId);
                _logger.LogInformation($"User {currentUserId} retrieved rooms for theater {theaterId}.");
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while retrieving rooms for theater {theaterId}: {ex.Message}");
                return BadRequest($"Có lỗi khi lấy danh sách phòng: {ex.Message}");
            }
        }

        [HttpDelete("DeleteRoom/{roomId}")]
        [Authorize]
        public IActionResult DeleteRoom(string roomId)
        {
            // Kiểm tra quyền xóa phòng
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteRoom))
            {
                _logger.LogWarning($"User {currentUserId} tried to delete room {roomId} without permission.");
                return Unauthorized("Bạn không có quyền xóa phòng.");
            }

            try
            {
                _roomService.DeleteRoom(roomId);
                _logger.LogInformation($"User {currentUserId} deleted room {roomId}.");
                return NoContent(); // Trả về status 204 (No Content)
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while deleting room {roomId}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateRoom/{roomId}")]
        [Authorize]
        public IActionResult UpdateRoom(string id, [FromBody] RoomDto dto)
        {
            // Kiểm tra quyền cập nhật phòng
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateRoom))
            {
                _logger.LogWarning($"User {currentUserId} tried to update room {id} without permission.");
                return Unauthorized("Bạn không có quyền cập nhật phòng.");
            }

            if (id != dto.Id)
            {
                _logger.LogWarning($"ID mismatch: URL ID {id} does not match body ID {dto.Id}.");
                return BadRequest("ID trong URL không khớp với ID trong dữ liệu.");
            }

            try
            {
                _roomService.UpdateRoom(dto);
                _logger.LogInformation($"Room {id} updated successfully by user {currentUserId}.");
                return Ok("Cập nhật phòng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while updating room {id}: {ex.Message}");
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }
}