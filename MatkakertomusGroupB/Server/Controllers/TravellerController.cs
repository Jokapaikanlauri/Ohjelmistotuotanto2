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
            _logger.LogInformation($"HttpPost PostGetNick Username = {userDTOInput.Username}");
            if (userDTOInput != null)
            {
                var traveller = await _context.Users.FirstAsync(x => x.UserName == userDTOInput.Username);
                userDTOInput.Nickname = traveller.Nickname.ToString();

                _logger.LogInformation($"HttpPost PostGetNick nickname = {userDTOInput.Nickname}");
                if (userDTOInput.Nickname != null)
                {
                    _logger.LogInformation("Ejecting user nickname embedded in userDTOInput");
                    return userDTOInput;
                }
            }
            _logger.LogWarning("HttpPost PostGetNick returning NotFound");
            return NotFound();
        }

        //[AllowAnonymous]
        [Route("id")]
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostGetId(UserDTO userDTOInput)
        {
            _logger.LogInformation($"HttpPost PostGetId Username = {userDTOInput.Username}");

			if (userDTOInput != null)
			{
				intParseOK = int.TryParse(_context.Users.First(x => x.UserName == userDTOInput.Username).Id, out parsedInt);
				userDTOInput.Id = parsedInt;

				_logger.LogInformation($"HttpPost PostGetId Id = {userDTOInput.Id}");


				if (intParseOK)
				{
					_logger.LogInformation("Ejecting user Id embedded in userDTOInput");
					return userDTOInput;
				}
				_logger.LogWarning("HttpPost PostGetId UserId parsing failed");
			}
			_logger.LogWarning("HttpPost PostGetId returning NotFound");
			return NotFound();
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Traveller>>> Get()
		{
			return await _context.Travellers.ToListAsync();
		}
	}
}

