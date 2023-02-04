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
    public class MatkakohdeController : ControllerBase
    {
        private readonly MatkaContext _context;

        public MatkakohdeController(MatkaContext context)
        {
            _context = context;
        }

        // GET: api/Matkakohde
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Matkakohde>>> GetMatkakohteet()
        {
          if (_context.Matkakohteet == null)
          {
              return NotFound();
          }
            return await _context.Matkakohteet.ToListAsync();
        }

        // GET: api/Matkakohde/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Matkakohde>> GetMatkakohde(int id)
        {
          if (_context.Matkakohteet == null)
          {
              return NotFound();
          }
            var matkakohde = await _context.Matkakohteet.FindAsync(id);

            if (matkakohde == null)
            {
                return NotFound();
            }

            return matkakohde;
        }

        // PUT: api/Matkakohde/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatkakohde(int id, Matkakohde matkakohde)
        {
            if (id != matkakohde.MatkakohdeId)
            {
                return BadRequest();
            }

            _context.Entry(matkakohde).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatkakohdeExists(id))
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

        // POST: api/Matkakohde
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Matkakohde>> PostMatkakohde(Matkakohde matkakohde)
        {
          if (_context.Matkakohteet == null)
          {
              return Problem("Entity set 'MatkaContext.Matkakohteet'  is null.");
          }
            _context.Matkakohteet.Add(matkakohde);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatkakohde", new { id = matkakohde.MatkakohdeId }, matkakohde);
        }

        // DELETE: api/Matkakohde/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatkakohde(int id)
        {
            if (_context.Matkakohteet == null)
            {
                return NotFound();
            }
            var matkakohde = await _context.Matkakohteet.FindAsync(id);
            if (matkakohde == null)
            {
                return NotFound();
            }

            _context.Matkakohteet.Remove(matkakohde);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MatkakohdeExists(int id)
        {
            return (_context.Matkakohteet?.Any(e => e.MatkakohdeId == id)).GetValueOrDefault();
        }
    }
}
