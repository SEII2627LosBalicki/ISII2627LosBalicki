using AppForSEII.API.DTOs.RefereeDTOs;
namespace AppForSEII.API.Controllers;



    [Route("api/[controller]")]
    [ApiController]

    public class RefereesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RefereesController> _logger;

        public RefereesController(ApplicationDbContext context, ILogger<RefereesController> logger)
        {
            _context = context;
            _logger=logger;
        }

        

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<RefereeForRefereeGroupDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRefereesForRefereeGroup(string? sportName,int? yearsRefereeing)
        {
            IList<RefereeForRefereeGroupDTO> referees = await _context.Referees
                .Include(r => r.SportForReferee)
                .Where(r => r.YearsRefereeing > 0
                    && (yearsRefereeing == null || r.YearsRefereeing >= yearsRefereeing)
                    && (sportName == null || r.SportForReferee!.Name == sportName))
                .OrderBy(r => r.Surname)
                    .ThenBy(r => r.Name)
                .Select(r => new RefereeForRefereeGroupDTO(r.Id, r.Name, r.Surname,
                    r.YearsRefereeing, r.Rating, r.SportForReferee!.Name))
                .ToListAsync();
            return Ok(referees);
        }
        
    }
