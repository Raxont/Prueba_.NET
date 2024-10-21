// Controllers/TransportController.cs
using Microsoft.AspNetCore.Mvc;
using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TransportController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transport>>> GetTransports()
        {
            return await _context.Transports.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transport>> GetTransport(int id)
        {
            var transport = await _context.Transports.FindAsync(id);

            if (transport == null)
            {
                return NotFound();
            }

            return transport;
        }

        [HttpPost]
        public async Task<ActionResult<Transport>> PostTransport(Transport transport)
        {
            _context.Transports.Add(transport);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransport), new { id = transport.Id }, transport);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransport(int id, Transport transport)
        {
            if (id != transport.Id)
            {
                return BadRequest();
            }

            _context.Entry(transport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Transports.Any(e => e.Id == id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransport(int id)
        {
            var transport = await _context.Transports.FindAsync(id);
            if (transport == null)
            {
                return NotFound();
            }

            _context.Transports.Remove(transport);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
