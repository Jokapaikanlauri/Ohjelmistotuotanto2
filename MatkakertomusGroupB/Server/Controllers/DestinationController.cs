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
using Microsoft.CodeAnalysis.CodeStyle;
using Azure.Messaging;

namespace MatkakertomusGroupB.Server.Controllers
{
	//Remember to add [AllowAnonymous] to methods you want accessible without being authenticated
	//Pages that require authentication also require the tag [Authorize] (Client side)
	[Authorize]
	//[AllowAnonymous]
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
        [AllowAnonymous]
		[HttpGet]
        public async Task<ActionResult<IEnumerable<DestinationDTO>>> GetDestinations()
        {
          if (_context.Destinations == null)
          {
              return NotFound();
          }
            return DestinationListToDestinationDTOList(await _context.Destinations.ToListAsync());
        }

        [AllowAnonymous]
        // GET: api/Destination/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DestinationDTO>> GetDestination(int id)
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

            return DestinationToDestinationDTO(destination);
        }

        // PUT: api/Destination/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDestination(int id, DestinationDTO destinationDTO)
        {
            if (_context.Destinations == null || _context.Stories == null)
            {
                return NotFound();
            }
            var stories = await _context.Stories.Where(x => x.DestinationId == id).ToListAsync();
            if (stories.Count > 0)
            {
                return BadRequest();
            }

                Destination destination = DestinationDTOToDestination(destinationDTO);
            if (id != destinationDTO.DestinationId)
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
           Destination destination = DestinationDTOToDestination(destinationDTO);
            if (_context.Destinations == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Destinations'  is null.");
          }


            _context.Destinations.Add(destination);
            
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetDestination", new { id = destination.DestinationId }, destination);
        }

        // DELETE: api/Destination/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDestination(int id)
        {
           
            if (_context.Destinations == null || _context.Stories == null)
            {
                return NotFound();
            }
            var stories = await _context.Stories.Where(x => x.DestinationId == id).ToListAsync();
            if (stories.Count > 0)
            {
                return BadRequest();
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

        private List<Destination> DestinationDTOListToDestinationList(List<DestinationDTO> destinationDTOList)
        {
            List<Destination> destinationList = new List<Destination>();
            foreach (DestinationDTO destinationDTO in destinationDTOList) destinationList.Add(DestinationDTOToDestination(destinationDTO));

            return destinationList; 
        }
        private List<DestinationDTO> DestinationListToDestinationDTOList(List<Destination> destinationList)
        {
            List<DestinationDTO> destinationDTOList = new List<DestinationDTO>();
            foreach (Destination destination in destinationList) destinationDTOList.Add(DestinationToDestinationDTO(destination));

            return destinationDTOList;
        }

        private Destination DestinationDTOToDestination(DestinationDTO destinationDTO)
        {
           
            Destination destination= new Destination
            {
                DestinationId = destinationDTO.DestinationId,
                Name = destinationDTO.Name,
                Country = destinationDTO.Country,
                Municipality = destinationDTO.Municipality,
                Description = destinationDTO.Description,
                Image = destinationDTO.Image
            };
            return destination;
        }

        private DestinationDTO DestinationToDestinationDTO(Destination destination)
        {
            DestinationDTO destinationDTO = new DestinationDTO();
            destinationDTO.DestinationId = destination.DestinationId;
            destinationDTO.Name = destination.Name;
            destinationDTO.Country = destination.Country;
            destinationDTO.Description = destination.Description;
            destinationDTO.Image = destination.Image;
            destinationDTO.Municipality = destination.Municipality;

            return destinationDTO;
        }
        private bool DestinationExists(int id)
        {
            return (_context.Destinations?.Any(e => e.DestinationId == id)).GetValueOrDefault();
        }
    }
}
