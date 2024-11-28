using CM.ApplicationService.Ticket.Abstracts;
using CM.Domain.Ticket;
using CM.Dtos.Ticket;
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

        [HttpPost("book")]
        public async Task<IActionResult> BookTicket([FromBody] CreateTicketDto request)
        {
            if (request == null || request.SeatIds == null || request.SeatIds.Count == 0)
                return BadRequest("Invalid request data!");

            try
            {
                var ticket = await _ticketService.BookTicketAsync(request.UserId, request.ShowtimeId, request.SeatIds);

                return Ok(new
                {
                    Message = "Ticket booked successfully!",
                    TicketId = ticket.Id,
                    TotalPrice = ticket.TotalPrice,
                    BookingDate = ticket.BookingDate
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        //[HttpPost("booktest")] 
        //public async Task<IActionResult> BookTicketTest([FromBody] CreateTicketDto request)
        //{
        //    if (request == null || request.SeatIds == null || request.SeatIds.Count == 0)
        //        return BadRequest("Invalid request data!");

        //    try
        //    {
        //         //await _ticketService.BookTicketAndCreatePaymentUrlAsync(HttpContext ,request.UserId, request.ShowtimeId, request.SeatIds);
        //        var paymentUrl =  _ticketService.BookTicketAndCreatePaymentUrlAsync(request.UserId, request.ShowtimeId, request.SeatIds, HttpContext);
        //        return Ok(new { Url = paymentUrl });
             
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { Error = ex.Message });
        //    }
        //}
    }
}

