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
    public class MatkaajaController : ControllerBase
    {
        private readonly MatkaContext _context;

        public MatkaajaController(MatkaContext context)
        {
            _context = context;
        }

        // GET: api/Matkaaja
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Matkaaja>>> GetMatkaajat()
        {
          if (_context.Matkaajat == null)
          {
              return NotFound();
          }
            return await _context.Matkaajat.ToListAsync();
        }

        // GET: api/Matkaaja/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Matkaaja>> GetMatkaaja(int id)
        {
          if (_context.Matkaajat == null)
          {
              return NotFound();
          }
            var matkaaja = await _context.Matkaajat.FindAsync(id);

            if (matkaaja == null)
            {
                return NotFound();
            }

            return matkaaja;
        }

        // PUT: api/Matkaaja/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatkaaja(int id, Matkaaja matkaaja)
        {
            if (id != matkaaja.MatkaajaId)
            {
                return BadRequest();
            }

            _context.Entry(matkaaja).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatkaajaExists(id))
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

        // POST: api/Matkaaja
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Matkaaja>> PostMatkaaja(Matkaaja matkaaja)
        {
          if (_context.Matkaajat == null)
          {
              return Problem("Entity set 'MatkaContext.Matkaajat'  is null.");
          }
            _context.Matkaajat.Add(matkaaja);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatkaaja", new { id = matkaaja.MatkaajaId }, matkaaja);
        }

        // DELETE: api/Matkaaja/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatkaaja(int id)
        {
            if (_context.Matkaajat == null)
            {
                return NotFound();
            }
            var matkaaja = await _context.Matkaajat.FindAsync(id);
            if (matkaaja == null)
            {
                return NotFound();
            }

            _context.Matkaajat.Remove(matkaaja);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MatkaajaExists(int id)
        {
            return (_context.Matkaajat?.Any(e => e.MatkaajaId == id)).GetValueOrDefault();
        }
    }
}
