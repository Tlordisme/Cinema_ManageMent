using CM.Domain.Ticket;
using CM.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CM.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly CMDbContext _context;

        public TicketController(CMDbContext context)
        {
            _context = context;
        }

        // GET: api/ticket
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CMTicket>>> GetTickets()
        {
            return await _context.Tickets.Include(t => t.User)
                                         .Include(t => t.Showtime)
                                         .Include(t => t.TicketSeats)
                                         .ThenInclude(ts => ts.Seat)
                                         .ToListAsync();
        }

        // GET: api/ticket/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CMTicket>> GetTicket(int id)
        {
            var ticket = await _context.Tickets.Include(t => t.User)
                                               .Include(t => t.Showtime)
                                               .Include(t => t.TicketSeats)
                                               .ThenInclude(ts => ts.Seat)
                                               .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return ticket;
        }

        // POST: api/ticket
        [HttpPost]
        public async Task<ActionResult<CMTicket>> PostTicket(CMTicket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
        }

        // PUT: api/ticket/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket(int id, CMTicket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/ticket/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
}
