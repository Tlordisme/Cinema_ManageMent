using CM.ApplicantService.Auth.Permission.Abstracts;
using CM.Application.Ticket.Services;
using CM.ApplicationService.Ticket.Abstracts;
using CM.ApplicationService.Ticket.Implements;
using CM.Domain.Auth;
using CM.Domain.Ticket;
using CM.Dtos.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Share.Constant.Permission;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketRepository _ticketRepository;
        private readonly IPermissionService _permissionService;
        private readonly ILogger<TicketController> _logger; 

        public TicketController(ITicketService ticketService, ITicketRepository ticketRepository, IPermissionService permissionService, ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _ticketRepository = ticketRepository;
            _permissionService = permissionService;
            _logger = logger;  
        }

        [HttpPost("BookTicket")]
        [Authorize]
        public async Task<IActionResult> BookTicket([FromBody] CreateTicketDto request)
        {
            if (request == null || request.seatIds == null || request.seatIds.Count == 0)
            {
                _logger.LogWarning("Booking failed: Invalid request data.");
                return BadRequest("Invalid request data!");
            }

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User ID not found in token.");
                    return Unauthorized(new { Message = "User ID not found in token." });
                }

                int userId = int.Parse(userIdClaim);

                // Kiểm tra quyền trước khi đặt vé
                if (!_permissionService.CheckPermission(userId, PermissionKey.BookTicketPermission))
                {
                    _logger.LogWarning("User {UserId} does not have permission to book tickets.", userId);
                    return Unauthorized("You do not have permission to book tickets.");
                }

                _logger.LogInformation("User {UserId} is attempting to book tickets.", userId);

                var ticket = await _ticketService.BookTicketAsync(userId, request, HttpContext);

                _logger.LogInformation("Ticket booked successfully by user {UserId}. Ticket ID: {TicketId}, Total Price: {TotalPrice}", userId, ticket.Id, ticket.TotalPrice);

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
                _logger.LogError(ex, "Error occurred while booking ticket.");
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
                    _logger.LogWarning("User {UserId} does not have permission to view ticket details.", currentUserId);
                    return Unauthorized("You do not have permission to view ticket details.");
                }

                _logger.LogInformation("User {UserId} is attempting to view details of ticket {TicketId}.", currentUserId, ticketId);

                var ticketDetails = await _ticketService.GetTicketDetailsAsync(ticketId);
                if (ticketDetails == null)
                {
                    _logger.LogWarning("Ticket {TicketId} not found or not paid yet.", ticketId);
                    return NotFound(new { message = "Ticket not found or not paid yet." });
                }

                return Ok(ticketDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching ticket details.");
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
                _logger.LogWarning("User {UserId} does not have permission to delete ticket {TicketId}.", currentUserId, ticketId);
                return Unauthorized("You do not have permission to delete tickets.");
            }

            _logger.LogInformation("User {UserId} is attempting to delete ticket {TicketId}.", currentUserId, ticketId);

            var result = await _ticketService.DeleteTicket(ticketId);
            if (result)
            {
                _logger.LogInformation("Ticket {TicketId} deleted successfully by user {UserId}.", ticketId, currentUserId);
                return NoContent();
            }

            _logger.LogWarning("Ticket {TicketId} not found for deletion by user {UserId}.", ticketId, currentUserId);
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
                _logger.LogInformation("User {UserId} is retrieving all tickets.", currentUserId);
                var allTickets = await _ticketService.GetAllTickets();
                return Ok(allTickets);
            }

            // Nếu không có quyền xem tất cả vé, kiểm tra quyền xem vé của chính mình
            if (_permissionService.CheckPermission(currentUserId, "ViewUserTickets"))
            {
                _logger.LogInformation("User {UserId} is retrieving their own tickets.", currentUserId);
                var userTickets = await _ticketService.GetTicketsByUserId(currentUserId);
                return Ok(userTickets);
            }

            _logger.LogWarning("User {UserId} does not have permission to view tickets.", currentUserId);
            return Unauthorized("You do not have permission to view tickets.");
        }
    }
}
