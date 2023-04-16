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
    public class StoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<StoryController> _logger;

        public StoryController(ApplicationDbContext context, ILogger<StoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Story
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryDTO>>> GetStories()
        {
            if (_context.Stories == null)
            {
                return NotFound();
            }
            return StoryListToStoryDTOList(await _context.Stories.ToListAsync());
        }

        // GET: api/Story/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoryDTO>> GetStory(int id)
        {
            if (_context.Stories == null)
            {
                return NotFound();
            }
            var story = await _context.Stories.FindAsync(id);

            if (story == null)
            {
                return NotFound();
            }

            return StoryToStoryDTO(story);
        }

        // GET: api/Story/trip/5
        // Get storylist with trip id
        [HttpGet("trip/{id}")]
        public async Task<ActionResult<IEnumerable<StoryDTO>>> GetTripStoryList(int id)
        {
            if (_context.Stories == null)
            {
                return NotFound();
            }
            List<Story>? list = null;
            list = await _context.Stories.Where(x => x.TripId == id).ToListAsync();

            if (list == null)
            {
                return NotFound();
            }

            return StoryListToStoryDTOList(list);
        }

        // PUT: api/Story/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStory(int id, StoryDTO storyDTO)
        {
            if (id != storyDTO.StoryId)
            {
                return BadRequest();
            }
            Story story = StoryDTOToStory(storyDTO);

            _context.Entry(story).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoryExists(id))
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

        // POST: api/Story
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StoryDTO>> PostStory(StoryDTO storyDTO)
        {
            if (_context.Stories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Stories'  is null.");
            }
            Story story = StoryDTOToStory(storyDTO);
            _context.Stories.Add(story);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStory", new { id = story.StoryId }, story);
        }

        // DELETE: api/Story/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStory(int id)
        {
            if (_context.Stories == null)
            {
                return NotFound();
            }
            var story = await _context.Stories.FindAsync(id);
            if (story == null)
            {
                return NotFound();
            }

            _context.Stories.Remove(story);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public List<Story> StoryDTOListToStoryList(List<StoryDTO> storyDTOList)
        {
            List<Story> storyList = new();
            foreach (StoryDTO storyDTO in storyDTOList) storyList.Add(StoryDTOToStory(storyDTO));
            return storyList;
        }

        public List<StoryDTO> StoryListToStoryDTOList(List<Story> storyList)
        {
            List<StoryDTO> storyDTOList = new();
            foreach (Story story in storyList) storyDTOList.Add(StoryToStoryDTO(story));
            return storyDTOList;
        }

        public Story StoryDTOToStory(StoryDTO storyDTO)
        {
            Story story = new Story();
            story.StoryId = storyDTO.StoryId;
            story.TripId = storyDTO.TripId;
            story.DestinationId = storyDTO.DestinationId;
            story.Description = storyDTO.Description;
            story.Datum = storyDTO.Datum;

            return story;
        }

        public StoryDTO StoryToStoryDTO(Story story)
        {
            StoryDTO storyDTO = new StoryDTO();
            storyDTO.StoryId = story.StoryId;
            storyDTO.TripId = story.TripId;
            storyDTO.DestinationId = story.DestinationId;
            storyDTO.Description = story.Description;
            storyDTO.Datum = story.Datum;

            return storyDTO;
        }

        private bool StoryExists(int id)
        {
            return (_context.Stories?.Any(e => e.StoryId == id)).GetValueOrDefault();
        }
    }
}
