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
                
            .Where(t => (captainName == null || (t.Captain != null && t.Captain.Name != null && t.Captain.Name.Contains(captainName))) 
                        && (sport == null || (t.Sport != null && t.Sport.Name != null && t.Sport.Name.Contains(sport))))
            .OrderBy(t => t.Name)
            .Select(t => new TeamForGameDTO(t.Id, t.Name, t.Captain.Name, t.Captain.Surname, t.TeamInvitations.Count(ti => ti.InvitationAccepted), t.MinAge, t.MaxAge, t.Sport.Name))
            .ToListAsync();
        return Ok(teams);
    }


    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(TeamDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetTeam(int id)
    {
        if (id < 0)
        {
            _logger.LogError($"Error: Team with id {id} is not valid");
            return NotFound();
        }

        var teamDTO = await _context.Teams
            .Where(t => t.Id == id)
            .Select(t => new TeamDetailDTO(
                t.Id,
                t.Name,
                t.Captain.Name,
                t.Captain.Surname,
                t.Sport.Name,
                t.MinAge,
                t.MaxAge,
                t.Captain.Age,
                t.TeamInvitations.Select(ti => new TeamInvitationDTO(
                    ti.User.Name,
                    ti.User.Surname,
                    ti.User.Id,
                    ti.User.Age,
                    ti.InvitationMessage)).ToList()))
            .FirstOrDefaultAsync();

        if (teamDTO == null)
        {
            _logger.LogError($"Error: Team with id {id} does not exist");
            return NotFound();
        }

        return Ok(teamDTO);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(TeamDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreateTeam(TeamForCreateDTO teamForCreateDTO)
    {

        //Alternative flow 3
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid Model");
        }


        //Alternative flow 3
        if (teamForCreateDTO.MinAge > teamForCreateDTO.MaxAge)            
        {
            _logger.LogError($"{DateTime.Now} Exception: Minimum age can't be bigger than maximum age");
            return BadRequest("minAge can't be greater than maxAge");
        }

        //Alternative flow 5
        if (await _context.Teams.AnyAsync(t => t.Name == teamForCreateDTO.Name))
        {
            _logger.LogError($"{DateTime.Now} Exception: Can't have the same name repeated");
            return BadRequest("Name can not be repeated");
        }
        
        //Step 5
        var sport = await _context.Sports.FirstOrDefaultAsync(s => s.Name == teamForCreateDTO.SportName);

        if (sport == null)
        {
            _logger.LogError($"{DateTime.Now} Exception: Sport doesn't exist");
            return NotFound("There is no sport with this name");
        }

        var captain = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == teamForCreateDTO.CaptainId);

        //Precondition
        if (captain == null)
        {
            _logger.LogError($"{DateTime.Now} Exception: Captain doesn't exist");
            return NotFound("There is no captain with this Id");
        }

        //Alternative flow 4
        if (teamForCreateDTO.InvitedPlayers.Count + 1 < sport.MinimumNumberofPlayers)
        {
            _logger.LogError($"{DateTime.Now} Exception: Not enough members");
            return BadRequest($"Less members than the MinimumNumberOfPlayers ({sport.MinimumNumberofPlayers}). Select more players");
        }

        //Step 5
        var invitedUsers = new List<(ApplicationUser User, string Message)>();

        foreach (var invitation in teamForCreateDTO.InvitedPlayers)
        {
            //Extra captain can't invite himself
            if (invitation.Id == teamForCreateDTO.CaptainId)
            {
                _logger.LogError($"{DateTime.Now} Exception: The captain can't be invited");
                return BadRequest("The captain can't be in the list of invited players");
            }

            //Extra: Player can't be invited twice
            if (invitedUsers.Any(i => i.User.Id == invitation.Id))
            {
                _logger.LogError($"{DateTime.Now} Exception: User {invitation.Id} invited more than once");
                return BadRequest($"The user {invitation.Id} is invited more than once");
            }

            //Step 3
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == invitation.Id);

            if (user == null)
            {
                _logger.LogError($"{DateTime.Now} Exception: Invited user {invitation.Id} doesn't exist");
                return NotFound($"There is no user with Id {invitation.Id}");
            }

            invitedUsers.Add((user, invitation.InvitationMessage));
        }

        //Extra Captain + invited can't exceed maximum number of players
        if (invitedUsers.Count + 1 > teamForCreateDTO.MaxMembers)
        {
            _logger.LogError($"{DateTime.Now} Exception: Too many members");
            return BadRequest($"Captain plus invited players exceed MaxMembers ({teamForCreateDTO.MaxMembers})");
        }

        //Step 6
        var team = new Team(0, teamForCreateDTO.Name, teamForCreateDTO.Description, teamForCreateDTO.MaxMembers,
            teamForCreateDTO.MinAge, teamForCreateDTO.MaxAge, sport, captain);

        //Post condition invitations are created
        foreach (var (invitedUser, message) in invitedUsers)
        {
            team.TeamInvitations.Add(new TeamInvitation(invitedUser, team, message));
        }

        //Post Condition team is saved
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        //Step 7: Show team created with its invited players
        var teamDetailDTO = new TeamDetailDTO(
            team.Id,
            team.Name,
            captain.Name,
            captain.Surname,
            sport.Name,
            team.MinAge,
            team.MaxAge,
            captain.Age,
            invitedUsers.Select(i => new TeamInvitationDTO(
                i.User.Name,
                i.User.Surname,
                i.User.Id,
                i.User.Age,
                i.Message)).ToList());


        //Step 7 201 to GetTeam
        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, teamDetailDTO);
    }
}
    