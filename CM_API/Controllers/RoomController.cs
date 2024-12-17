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

        public RoomController(IRoomService roomService, IPermissionService permissionService)
        {
            _roomService = roomService;
            _permissionService = permissionService;
        }

        [HttpPost("AddRoom")]
        [Authorize]
        public IActionResult CreateRoom([FromBody] RoomDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.AddRoom))
            {
                return Unauthorized("Bạn không có quyền thêm phòng.");
            }

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

        [HttpGet("GetAllRooms/{theaterId}")]
        [Authorize]
        public IActionResult GetRoomsByTheaterId(string theaterId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewRoomsByTheater))
            {
                return Unauthorized("Bạn không có quyền xem danh sách phòng của rạp.");
            }

            try
            {
                var rooms = _roomService.GetRoomsByTheaterId(theaterId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return BadRequest($"Có lỗi khi lấy danh sách phòng: {ex.Message}");
            }
        }

        [HttpDelete("DeleteRoom/{roomId}")]
        [Authorize]
        public IActionResult DeleteRoom(string roomId)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteRoom))
            {
                return Unauthorized("Bạn không có quyền xóa phòng.");
            }

            try
            {
                _roomService.DeleteRoom(roomId);
                return NoContent(); // Trả về status 204 (No Content)
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateRoom/{roomId}")]
        [Authorize]
        public IActionResult UpdateRoom(string id, [FromBody] RoomDto dto)
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateRoom))
            {
                return Unauthorized("Bạn không có quyền cập nhật phòng.");
            }

            if (id != dto.Id)
            {
                return BadRequest("ID trong URL không khớp với ID trong dữ liệu.");
            }

            try
            {
                _roomService.UpdateRoom(dto);
                return Ok("Cập nhật phòng thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
    }
}
