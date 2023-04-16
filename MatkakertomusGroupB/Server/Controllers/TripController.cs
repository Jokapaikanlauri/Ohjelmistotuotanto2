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
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TripController> _logger;

        public TripController(ApplicationDbContext context, ILogger<TripController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Trip 
        // this gets all the public trips
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTrip()
        {
            if (_context.Trip == null)
            {
                return NotFound();
            }
            return await _context.Trip.Where(x => x.Private == false).ToListAsync();
        }

        // GET: api/Trip/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Trip>> GetTrip(int id)
        {
            if (_context.Trip == null)
            {
                return NotFound();
            }
            var trip = await _context.Trip.FindAsync(id);

            if (trip == null)
            {
                return NotFound();
            }

            return trip;
        }

        // GET: api/Trip/traveller/idString
        // Get all trips with traveller id
        [HttpGet("traveller/{id}")]
        public async Task<ActionResult<IEnumerable<Trip>>> GetTravellerTrip(string id)
        {
            if (_context.Trip == null)
            {
                return NotFound();
            }

            List<Trip> list = await _context.Trip.Where(x => x.Traveller.Id == id).ToListAsync();

            if (list == null)
            {
                return NotFound();
            }

            return list;
        }

        // PUT: api/Trip/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrip(int id, TripDTO tripDTO)
        {

            //Convert DTO to Trip
            Trip trip = TripDTOtoTrip(tripDTO);
            trip.TripId = id;

            if (id != trip.TripId)
            {
                return BadRequest();
            }


            _context.Entry(trip).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TripExists(id))
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

        // POST: api/Trip
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Trip>> PostTrip(TripDTO tripDTO)
        {
            if (_context.Trip == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Trip'  is null.");
            }

            //Convert DTO to Trip
            Trip trip = TripDTOtoTrip(tripDTO);

            //Add trip
            _context.Trip.Add(trip);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrip", new { id = trip.TripId }, trip);
        }

        // DELETE: api/Trip/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrip(int id)
        {
            if (_context.Trip == null)
            {
                return NotFound();
            }
            var trip = await _context.Trip.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            _context.Trip.Remove(trip);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private Trip TripDTOtoTrip(TripDTO tripDTO)
        {
            //Convert DTO to Trip
            Trip trip = new Trip
            {
                TravellerId = tripDTO.TravellerId,
                DatumStart = tripDTO.DatumStart,
                DatumEnd = tripDTO.DatumEnd,
                Private = tripDTO.Private
            };
            return trip;
        }

        private bool TripExists(int id)
        {
            return (_context.Trip?.Any(e => e.TripId == id)).GetValueOrDefault();
        }
    }
}
