using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;
using AppForSEII.API.Data;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

     ApplicationUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApplicationUsersController> _logger;

        public ApplicationUsersController(ApplicationDbContext context, 
            ILogger<ApplicationUsersController> logger)
        {
            _context = context;
            _logger=logger;
        }

        // GET: ApplicationUsers
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<ApplicationUserDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Index()
        {
            return Ok(await _context.ApplicationUsers
            .Select(u=>new ApplicationUserDTO(u.Id, u.Name, u.Surname, u.UserName, u.PhoneNumber))
            .ToListAsync());
        }

        // GET: ApplicationUsers/Details/5
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(ApplicationUserDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUserDTO = await _context.ApplicationUsers
                .Where(u => u.Id == id)
                .Select(u=>new ApplicationUserDTO(u.Id, u.Name, u.Surname, u.UserName, u.PhoneNumber))
                .FirstOrDefaultAsync();

            if (applicationUserDTO == null)
            {
                _logger.LogError($"Error: ApplicationUser with id {id} does not exist");

                return NotFound();
            }

            return Ok(applicationUserDTO);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<UserForTeamDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetUsersForTeam(string? sportName, int? minAge, int? maxAge)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            DateOnly? maxBirthDate = minAge.HasValue ? today.AddYears(-minAge.Value) : null;
            DateOnly? minBirthDate = maxAge.HasValue ? today.AddYears(-(maxAge.Value + 1)) : null;

            var users = await _context.ApplicationUsers
                .Include(u => u.UserInterested)
                    .ThenInclude(i => i.Sport)
                .Where(u => (maxBirthDate == null || u.BirthDate <= maxBirthDate.Value)
                        && (minBirthDate == null || u.BirthDate > minBirthDate.Value)
                        && (sportName == null || u.UserInterested.Any(i => i.Sport.Name == sportName)))
                .OrderBy(u => u.Surname)
                    .ThenBy(u => u.Name)
                .Select(u => new UserForTeamDTO(
                    u.Id,
                    u.Name,
                    u.Surname,
                    u.Age,
                    u.UserInterested.Select(i => new SportSkillDTO(i.Sport.Name, i.Skill)).ToList()))
                .ToListAsync();

            return Ok(users);
        }


        private bool ApplicationUserDTOExists(string name, string surname)
        {
            return _context.ApplicationUsers.Any(u => u.Name == name && u.Surname==surname);
        }
    }
}
