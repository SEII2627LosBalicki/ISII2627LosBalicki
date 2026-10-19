using AppForSEII.API.DTOs.TeamDTOs;
using AppForSEII.API.DTOs.GameDTOs;

namespace AppForSEII.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public sealed class GamesController : ControllerBase
{
    private readonly ILogger<GamesController> _logger;
    private readonly ApplicationDbContext _context;

    public GamesController(ILogger<GamesController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
        

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(GameForDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetGame(int id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var gameDTO = await _context.Games
            .Where(g => g.Id == id)
            .Select(g => new GameForDetailDTO(g.Id, g.Name, g.Sport.Name, g.Date, g.Place, g.Description, 
                    g.GameInvitations.Count,
                    g.GameInvitations.Select(gi => new TeamForGameDetailDTO(gi.TeamInvited.Id, gi.TeamInvited.Name, gi.TeamInvited.Captain.Name, gi.TeamInvited.Captain.Surname, 
                                                                            gi.TeamInvited.TeamInvitations.Count(ti => ti.InvitationAccepted), gi.TeamInvited.MinAge, gi.TeamInvited.MaxAge, 
                                                                            gi.TeamInvited.Sport.Name, gi.Message))
                    .ToList()
            ))            
            .FirstOrDefaultAsync();

        if (gameDTO == null)
        {
            _logger.LogError($"Error: Game with id {id} does not exist");

            return NotFound();
        }

        return Ok(gameDTO);        
    }
}