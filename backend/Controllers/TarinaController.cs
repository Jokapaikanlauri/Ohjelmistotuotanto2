using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarinaController : ControllerBase
    {
        private readonly MatkaContext _context;

        public TarinaController(MatkaContext context)
        {
            _context = context;
        }

        // GET: api/Tarina
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tarina>>> GetTarinat()
        {
          if (_context.Tarinat == null)
          {
              return NotFound();
          }
            return await _context.Tarinat.ToListAsync();
        }

        // GET: api/Tarina/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tarina>> GetTarina(int id)
        {
          if (_context.Tarinat == null)
          {
              return NotFound();
          }
            var tarina = await _context.Tarinat.FindAsync(id);

            if (tarina == null)
            {
                return NotFound();
            }

            return tarina;
        }

        // PUT: api/Tarina/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTarina(int id, Tarina tarina)
        {
            if (id != tarina.TarinaId)
            {
                return BadRequest();
            }

            _context.Entry(tarina).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TarinaExists(id))
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

        // POST: api/Tarina
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tarina>> PostTarina(Tarina tarina)
        {
          if (_context.Tarinat == null)
          {
              return Problem("Entity set 'MatkaContext.Tarinat'  is null.");
          }
            _context.Tarinat.Add(tarina);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTarina", new { id = tarina.TarinaId }, tarina);
        }

        // DELETE: api/Tarina/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarina(int id)
        {
            if (_context.Tarinat == null)
            {
                return NotFound();
            }
            var tarina = await _context.Tarinat.FindAsync(id);
            if (tarina == null)
            {
                return NotFound();
            }

            _context.Tarinat.Remove(tarina);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TarinaExists(int id)
        {
            return (_context.Tarinat?.Any(e => e.TarinaId == id)).GetValueOrDefault();
        }
    }
}
