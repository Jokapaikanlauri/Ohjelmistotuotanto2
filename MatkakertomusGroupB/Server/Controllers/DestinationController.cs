using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MatkakertomusGroupB.Server.Data;
using MatkakertomusGroupB.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace MatkakertomusGroupB.Server.Controllers
{
    //Remember to add [AllowAnonymous] to methods you want accessible without being authenticated
    //Pages that require authentication also require the tag [Authorize] (Client side)
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DestinationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
		private readonly ILogger<DestinationController> _logger;

		public DestinationController(ApplicationDbContext context, ILogger<DestinationController> logger)
		{
			_context = context;
			_logger = logger;
		}

		// GET: api/Destination
		[HttpGet]
        public async Task<ActionResult<IEnumerable<Destination>>> GetDestinations()
        {
          if (_context.Destinations == null)
          {
              return NotFound();
          }
            return await _context.Destinations.ToListAsync();
        }

        // GET: api/Destination/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Destination>> GetDestination(int id)
        {
          if (_context.Destinations == null)
          {
              return NotFound();
          }
            var destination = await _context.Destinations.FindAsync(id);

            if (destination == null)
            {
                return NotFound();
            }

            return destination;
        }

        // PUT: api/Destination/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDestination(int id, Destination destination)
        {
            if (id != destination.DestinationId)
            {
                return BadRequest();
            }

            _context.Entry(destination).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DestinationExists(id))
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

        // POST: api/Destination
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Destination>> PostDestination(DestinationDTO destinationDTO)
        {
          if (_context.Destinations == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Destinations'  is null.");
          }

            Destination destination = new Destination
            {
                Name = destinationDTO.Name,
                Country = destinationDTO.Country,
                Municipality = destinationDTO.Municipality,
                Description = destinationDTO.Description,
                Image = destinationDTO.Image
            };
            _context.Destinations.Add(destination);
            
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetDestination", new { id = destination.DestinationId }, destination);
        }

        // DELETE: api/Destination/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            if (_context.Destinations == null)
            {
                return NotFound();
            }
            var destination = await _context.Destinations.FindAsync(id);
            if (destination == null)
            {
                return NotFound();
            }

            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DestinationExists(int id)
        {
            return (_context.Destinations?.Any(e => e.DestinationId == id)).GetValueOrDefault();
        }
    }
}
