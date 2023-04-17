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
    public class PictureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PictureController> _logger;

        public PictureController(ApplicationDbContext context, ILogger<PictureController> logger)
        {
            _context = context;
            _logger = logger;
        }


        // GET: api/Picture
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PictureDTO>>> GetPictures()
        {
            if (_context.Pictures == null)
            {
                return NotFound();
            }
            return PictureListToPictureDTOList(await _context.Pictures.ToListAsync());
        }

        // GET: api/Picture/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PictureDTO>> GetPicture(int id)
        {
            if (_context.Pictures == null)
            {
                return NotFound();
            }
            var picture = await _context.Pictures.FindAsync(id);

            if (picture == null)
            {
                return NotFound();
            }

            return PictureToPictureDTO(picture);
        }

        // Get pictures by story id
        // GET: api/Picture/s5
        [HttpGet("story/{id}")]
        public async Task<ActionResult<IEnumerable<PictureDTO>>> GetStoryPictures(int id)
        {
            if (_context.Pictures == null)
            {
                return NotFound();
            }
            List<Picture>? list = null;
            list = await _context.Pictures.Where(x => x.StoryId == id).ToListAsync();

            if (list == null)
            {
                return NotFound();
            }

            return PictureListToPictureDTOList(list);
        }

        // PUT: api/Picture/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPicture(int id, PictureDTO pictureDTO)
        {
            Picture picture = PictureDTOToPicture(pictureDTO);
            if (id != picture.PictureId)
            {
                return BadRequest();
            }

            _context.Entry(picture).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PictureExists(id))
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

        // POST: api/Picture
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{id}")]
        public async Task<ActionResult<PictureDTO>> PostPicture(int id, PictureDTO pictureDTO)
        {
            Picture picture = new Picture();
            picture.Image = pictureDTO.Image;
            picture.StoryId = id;

            if (_context.Pictures == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pictures'  is null.");
            }
            _context.Pictures.Add(picture);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPicture", new { id = picture.PictureId }, picture);
        }

        // DELETE: api/Picture/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePicture(int id)
        {
            if (_context.Pictures == null)
            {
                return NotFound();
            }
            var picture = await _context.Pictures.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }

            _context.Pictures.Remove(picture);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // delete all pictures from a certain story
        // DELETE: api/Picture/story/5
        [HttpDelete("/story/{id}")]
        public async Task<IActionResult> DeleteStoryPictures(int storyId)
        {
            if (_context.Pictures == null)
            {
                return NotFound();
            }

            IEnumerable<Picture> pictureList = (IEnumerable<Picture>)GetStoryPictures(storyId);

            if (pictureList == null)
            {
                return NotFound();
            }

            foreach (Picture picture in pictureList)
            {
                // delete each picture in picturelist
                _context.Pictures.Remove(picture);
            }
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private List<Picture> PictureDTOListToPictureList(List<PictureDTO> pictureDTOList) 
        {
            List<Picture> pictureList = new List<Picture>();
            foreach (PictureDTO pictureDTO in pictureDTOList) pictureList.Add(PictureDTOToPicture(pictureDTO));
            
            return pictureList;
        }

        private List<PictureDTO> PictureListToPictureDTOList(List<Picture> pictureList) 
        {
            List<PictureDTO> pictureDTOList = new List<PictureDTO>();
            foreach(Picture picture in pictureList) pictureDTOList.Add(PictureToPictureDTO(picture));

            return pictureDTOList;
        }

        private Picture PictureDTOToPicture(PictureDTO pictureDTO)
        {
            Picture picture = new Picture();
            if (pictureDTO.PictureId != null) picture.PictureId = Convert.ToInt32(pictureDTO.PictureId);
            picture.StoryId = pictureDTO.StoryId;
            pictureDTO.Image = picture.Image;

            return picture;
        }

        private PictureDTO PictureToPictureDTO(Picture picture)
        {
            PictureDTO pictureDTO = new PictureDTO();
            pictureDTO.PictureId = picture.PictureId;
            pictureDTO.StoryId = picture.StoryId;
            pictureDTO.Image = picture.Image;

            return pictureDTO;
        }


        private bool PictureExists(int id)
        {
            return (_context.Pictures?.Any(e => e.PictureId == id)).GetValueOrDefault();
        }
    }
}
