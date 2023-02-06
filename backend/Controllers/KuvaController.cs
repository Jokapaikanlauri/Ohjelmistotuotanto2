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
    public class KuvaController : ControllerBase
    {
        private readonly MatkaContext _context;

        public KuvaController(MatkaContext context)
        {
            _context = context;
        }

        // GET: api/Kuva
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kuva>>> GetKuvat()
        {
          if (_context.Kuvat == null)
          {
              return NotFound();
          }
            return await _context.Kuvat.ToListAsync();
        }

        // GET: api/Kuva/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Kuva>> GetKuva(int id)
        {
          if (_context.Kuvat == null)
          {
              return NotFound();
          }
            var kuva = await _context.Kuvat.FindAsync(id);

            if (kuva == null)
            {
                return NotFound();
            }

            return kuva;
        }

        // PUT: api/Kuva/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutKuva(int id, Kuva kuva)
        {
            if (id != kuva.KuvaId)
            {
                return BadRequest();
            }

            _context.Entry(kuva).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!KuvaExists(id))
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

        // POST: api/Kuva
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Kuva>> PostKuva(Kuva kuva)
        {
          if (_context.Kuvat == null)
          {
              return Problem("Entity set 'MatkaContext.Kuvat'  is null.");
          }
            _context.Kuvat.Add(kuva);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetKuva", new { id = kuva.KuvaId }, kuva);
        }

        // DELETE: api/Kuva/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKuva(int id)
        {
            if (_context.Kuvat == null)
            {
                return NotFound();
            }
            var kuva = await _context.Kuvat.FindAsync(id);
            if (kuva == null)
            {
                return NotFound();
            }

            _context.Kuvat.Remove(kuva);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool KuvaExists(int id)
        {
            return (_context.Kuvat?.Any(e => e.KuvaId == id)).GetValueOrDefault();
        }
    }
}
