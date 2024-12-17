using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Seat.Abstracts;
using CM.Domain.Auth;
using CM.Dtos.Seat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

[Route("api/[controller]")]
[ApiController]
public class SeatPriceController : ControllerBase
{
    private readonly ISeatPriceService _seatPriceService;
    private readonly IPermissionService _permissionService;

    public SeatPriceController(ISeatPriceService seatPriceService, IPermissionService permissionService)
    {
        _seatPriceService = seatPriceService;
        _permissionService = permissionService;
    }

    [HttpPost("AddSeatPrice")]
    [Authorize]
    public IActionResult AddSeatPrice([FromBody] AddSeatPriceDto seatPriceDto)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.AddSeatPrice))
        {
            return Unauthorized("Bạn không có quyền thêm giá ghế.");
        }

        try
        {
            _seatPriceService.AddSeatPrice(seatPriceDto); // Pass user id to service for logging
            return Ok("Thêm giá ghế thành công!");
        }
        catch (Exception ex)
        {
            return BadRequest($"Có lỗi khi thêm giá ghế: {ex.Message}");
        }
    }

    [HttpPut("UpdateSeatPrice")]
    [Authorize]
    public IActionResult UpdateSeatPrice([FromBody] UpdateSeatPriceDto seatPriceDto)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.UpdateSeatPrice))
        {
            return Unauthorized("Bạn không có quyền cập nhật giá ghế.");
        }

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

    [HttpDelete("DeleteSeatPrice/{seatPriceId}")]
    [Authorize]
    public IActionResult DeleteSeatPrice(int seatPriceId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.DeleteSeatPrice))
        {
            return Unauthorized("Bạn không có quyền xóa giá ghế.");
        }

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

    [HttpGet("GetPriceOfTypeSeat/{roomId}")]
    [Authorize]
    public IActionResult GetSeatPricesByRoomId(string roomId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewSeatPricesByRoom))
        {
            return Unauthorized("Bạn không có quyền xem danh sách giá ghế theo phòng.");
        }

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


    [HttpGet("GetPriceOfType/{seatPriceId}")]
    [Authorize]
    public IActionResult GetSeatPrice(int seatPriceId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewSeatPriceById))
        {
            return Unauthorized("Bạn không có quyền xem giá ghế theo ID.");
        }

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
