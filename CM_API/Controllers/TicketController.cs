using CM.ApplicationService.Ticket.Abstracts;
using CM.Domain.Ticket;
using CM.Dtos.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        //        // Lấy UserId từ JWT token
        //        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        //                if (string.IsNullOrEmpty(userIdClaim))
        //                {
        //                    return Unauthorized(new { Message = "User ID not found in token." });
        //                }
        //int userId = int.Parse(userIdClaim);

        //// Thêm bình luận vào cơ sở dữ liệu
        //await _commentService.AddCommentAsync(request, userId);

        //return Ok(new { Message = "Comment added successfully." });

        [HttpPost("book")]
        [Authorize]
        public async Task<IActionResult> BookTicket([FromBody] CreateTicketDto request)
        {
            if (request == null || request.seatIds == null || request.seatIds.Count == 0)
                return BadRequest("Invalid request data!");

            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { Message = "User ID not found in token." });
                }
                int userId = int.Parse(userIdClaim);

                var ticket = await _ticketService.BookTicketAsync(
                    userId,
                    request,
                    HttpContext
                );

                return Ok(
                    new
                    {
                        Message = "Ticket booked successfully!",
                        TicketId = ticket.Id,
                        TotalPrice = ticket.TotalPrice,
                        BookingDate = ticket.BookingDate,
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
