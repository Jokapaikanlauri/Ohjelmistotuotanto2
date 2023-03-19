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
    public class TravellerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<TravellerController> _logger;

        public TravellerController(ApplicationDbContext context, ILogger<TravellerController> logger)
        {
            _context = context;
            _logger = logger;
        }

		[Route("nick")]
		[HttpPost]
		public async Task<ActionResult<UserDTO>> PostGetNick(UserDTO userDTOInput)
		{
			if (userDTOInput != null)
			{
				var traveller = await _context.Users.FirstAsync(x => x.UserName == userDTOInput.Username);
				userDTOInput.Nickname = traveller.Nickname.ToString();
				if (userDTOInput.Nickname != null)
				{
					return userDTOInput;
				}
			}
			return NotFound();
		}

		//[AllowAnonymous]
		[Route("id")]
		[HttpPost]
		public async Task<ActionResult<UserDTO>> PostGetId(UserDTO userDTOInput)
		{
			if (userDTOInput != null)
			{
				var user = await _context.Users.FirstAsync(x => x.UserName == userDTOInput.Username);
				userDTOInput.Id = user.Id;
				return userDTOInput;
			}
			return NotFound();
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Traveller>>> Get()
		{
			return await _context.Travellers.ToListAsync();
		}
	}
}

