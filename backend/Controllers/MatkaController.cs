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
    public class MatkaController : ControllerBase
    {
        private readonly MatkaContext _context;

        public MatkaController(MatkaContext context)
        {
            _context = context;
        }

        // GET: api/Matka
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Matka>>> GetMatkat()
        {
          if (_context.Matkat == null)
          {
              return NotFound();
          }
            return await _context.Matkat.ToListAsync();
        }

        // GET: api/Matka/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Matka>> GetMatka(int id)
        {
          if (_context.Matkat == null)
          {
              return NotFound();
          }
            var matka = await _context.Matkat.FindAsync(id);

            if (matka == null)
            {
                return NotFound();
            }

            return matka;
        }

        // PUT: api/Matka/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatka(int id, Matka matka)
        {
            if (id != matka.MatkaId)
            {
                return BadRequest();
            }

            _context.Entry(matka).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatkaExists(id))
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

        // POST: api/Matka
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Matka>> PostMatka(Matka matka)
        {
          if (_context.Matkat == null)
          {
              return Problem("Entity set 'MatkaContext.Matkat'  is null.");
          }
            _context.Matkat.Add(matka);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMatka", new { id = matka.MatkaId }, matka);
        }

        // DELETE: api/Matka/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatka(int id)
        {
            if (_context.Matkat == null)
            {
                return NotFound();
            }
            var matka = await _context.Matkat.FindAsync(id);
            if (matka == null)
            {
                return NotFound();
            }

            _context.Matkat.Remove(matka);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MatkaExists(int id)
        {
            return (_context.Matkat?.Any(e => e.MatkaId == id)).GetValueOrDefault();
        }
    }
}
