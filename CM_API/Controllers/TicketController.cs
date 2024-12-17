using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.ApplicationService.Ticket.Abstracts;
using CM.Domain.Auth;
using CM.Dtos.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

[Route("api/[controller]")]
[ApiController]
public class TicketController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IPermissionService _permissionService;

    public TicketController(ITicketService ticketService, IPermissionService permissionService)
    {
        _ticketService = ticketService;
        _permissionService = permissionService;
    }

    [HttpPost("BookTicket")]
    [Authorize]
    public async Task<IActionResult> BookTicket([FromBody] CreateTicketDto request)
    {
        if (request == null || request.seatIds == null || request.seatIds.Count == 0)
        {
            return BadRequest("Invalid request data!");
        }

        try
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new { Message = "User ID not found in token." });
            }

            int userId = int.Parse(userIdClaim);

            // Kiểm tra quyền trước khi đặt vé
            if (!_permissionService.CheckPermission(userId, PermissionKey.BookTicketPermission))
            {
                return Unauthorized("You do not have permission to book tickets.");
            }

            var ticket = await _ticketService.BookTicketAsync(userId, request, HttpContext);

            return Ok(new
            {
                Message = "Ticket booked successfully!",
                TicketId = ticket.Id,
                TotalPrice = ticket.TotalPrice,
                BookingDate = ticket.BookingDate,
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("GetDetailTicket/{ticketId}")]
    public async Task<IActionResult> GetTicketDetails(int ticketId)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirst("Id")?.Value);
            if (!_permissionService.CheckPermission(currentUserId, PermissionKey.ViewTicketDetails))
            {
                return Unauthorized("You do not have permission to view ticket details.");
            }

            var ticketDetails = await _ticketService.GetTicketDetailsAsync(ticketId);
            if (ticketDetails == null)
            {
                return NotFound(new { message = "Ticket not found or not paid yet." });
            }

            return Ok(ticketDetails);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("DeleteTicket/{ticketId}")]
    [Authorize]
    public async Task<IActionResult> DeleteTicket(int ticketId)
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        // Kiểm tra quyền xóa vé
        if (!_permissionService.CheckPermission(currentUserId, "DeleteTicket"))
        {
            return Unauthorized("You do not have permission to delete tickets.");
        }

        var result = await _ticketService.DeleteTicket(ticketId);
        if (result)
        {
            return NoContent();
        }

        return NotFound();
    }

    [HttpGet("GetAllTickets")]
    [Authorize]
    public async Task<IActionResult> GetAllTickets()
    {
        var currentUserId = int.Parse(User.FindFirst("Id")?.Value);

        // Kiểm tra quyền xem tất cả vé
        if (_permissionService.CheckPermission(currentUserId, "ViewAllTickets"))
        {
            var allTickets = await _ticketService.GetAllTickets();
            return Ok(allTickets);
        }

        // Nếu không có quyền xem tất cả vé, kiểm tra quyền xem vé của chính mình
        if (_permissionService.CheckPermission(currentUserId, "ViewUserTickets"))
        {
            var userTickets = await _ticketService.GetTicketsByUserId(currentUserId);
            return Ok(userTickets);
        }

        return Unauthorized("You do not have permission to view tickets.");
    }
}
