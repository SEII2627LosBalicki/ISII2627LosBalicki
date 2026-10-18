using Microsoft.AspNetCore.Mvc;
using System.Net; 
using Microsoft.Extensions.Logging;
using AppForSEII.API.DTOs.TeamDTOs;

namespace AppForSEII.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public sealed class TeamsController : ControllerBase
{
    private readonly ILogger<TeamsController> _logger;
    private readonly ApplicationDbContext _context;

    public TeamsController(ILogger<TeamsController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<TeamForGameDTO>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult> GetTeamsForGame(string? captainName, string? sport)
    {
        IList<TeamForGameDTO> teams = await _context.Teams
            .Include(t => t.Sport)
            .Include(t => t.TeamInvitations)
            .Include(t => t.Captain)
                
            .Where(t => (t.Captain.Name.Contains(captainName) || (captainName == null)) 
                     && ( t.Sport.Name.Contains(sport) || (sport == null)))
            .OrderBy(t => t.Name)
            .Select(t => new TeamForGameDTO(t.Id, t.Name, t.Captain.Name, t.Captain.Surname, t.TeamInvitations.Count(ti => ti.InvitationAccepted), t.MinAge, t.MaxAge, t.Sport.Name))
            .ToListAsync();
        return Ok(teams);
    }
}
    