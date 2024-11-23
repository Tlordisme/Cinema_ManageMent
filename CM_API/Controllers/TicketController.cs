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

        // Tạo vé mới
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromForm] CreateTicketDto createTicketDto)
        {
            if (createTicketDto == null)
                return BadRequest("Invalid ticket data.");

            var ticketDto = await _ticketService.CreateTicketAsync(createTicketDto);

            if (ticketDto == null)
                return BadRequest("Failed to create ticket.");

            return CreatedAtAction(nameof(GetTicketById), new { ticketId = ticketDto.Id }, ticketDto);
        }

        // Xóa vé
        [HttpDelete("{ticketId}")]
        public async Task<IActionResult> DeleteTicket(int ticketId)
        {
            var result = await _ticketService.DeleteTicketAsync(ticketId);

            if (!result)
                return NotFound("Ticket not found.");

            return NoContent();  // 204 No Content
        }

        // Lấy thông tin vé theo ID
        [HttpGet("{ticketId}")]
        public async Task<IActionResult> GetTicketById(int ticketId)
        {
            var ticketDto = await _ticketService.GetTicketByIdAsync(ticketId);

            if (ticketDto == null)
                return NotFound("Ticket not found.");

            return Ok(ticketDto);
        }

        // Xử lý thanh toán
        [HttpPost("process-payment/{ticketId}")]
        public async Task<IActionResult> ProcessPayment(int ticketId)
        {
            var result = await _ticketService.ProcessPaymentAsync(ticketId);

            if (!result)
                return BadRequest("Payment processing failed.");

            return Ok("Payment processed successfully.");
        }

        // Cập nhật trạng thái vé
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateTicketStatus([FromBody] UpdateTicketDto updateTicketDto)
        {
            if (updateTicketDto == null)
                return BadRequest("Invalid ticket status data.");

            var result = await _ticketService.UpdateTicketStatusAsync(updateTicketDto);

            if (!result)
                return NotFound("Ticket not found.");

            return NoContent();  // 204 No Content
        }
    }
}

