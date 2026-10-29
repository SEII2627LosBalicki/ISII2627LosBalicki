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

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(GameForDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> CreateGame(GameForCreateDTO gameForCreate)
    {
        //INITIAL VALIDATIONS

        // Condition Step 5
        if(gameForCreate.Date <= DateTime.Now)
        {
            ModelState.AddModelError("Date", "Error! Your game date is in the past.");            
        }
        
        // Condition Step 5
        if (await _context.Games.AnyAsync(g => g.Name == gameForCreate.Name))
        {
            ModelState.AddModelError("Name", "Error! Game with this name already exists.");
        }

        // Alternative Flow 3
        if(gameForCreate.GameInvitations.Count < 2)
        {
            ModelState.AddModelError("GameInvitations", "Error! You must invite at least 2 teams to the game.");
        }

        // Post-condition
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == gameForCreate.ResponsibleUserName);
        if (user == null)
        {
            ModelState.AddModelError("ResponsibleUserName", "Error! Responsible user is not registered.");
        }

        // Condition Step 7
        var sport = await _context.Sports.FirstOrDefaultAsync(s => s.Name == gameForCreate.SportName);
        if (sport == null)
        {
            ModelState.AddModelError("SportName", "Error! Sport does not exist.");
        }

        if(ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        // COMPLEX VALIDATIONS

        // We need to check if the same team is invited more than once. We count the number of distinct team IDs and compare it with 
        // the total number of team IDs. If they are not equal, it means that at least one team has been invited more than once.
        var teamIDs = gameForCreate.GameInvitations.Select(gi => gi.TeamId).ToList(); // Get all the team IDs from the game invitations

        if(teamIDs.Distinct().Count() != teamIDs.Count())
        {
            ModelState.AddModelError("GameInvitations", "Error! A team cannot be invited more than once.");
        }

        // Condition Step 4
        var teams = await _context.Teams
            .Include(t => t.Captain)
            .Include(t => t.Sport)
            .Include(t => t.TeamInvitations)

            .Where(t => teamIDs.Contains(t.Id))
            .ToListAsync();

        foreach (var invitation in gameForCreate.GameInvitations)
        {
            var team = teams.FirstOrDefault(t => t.Id == invitation.TeamId);
            if(team == null)
            {
                ModelState.AddModelError("GameInvitations", $"Error! Team with ID {invitation.TeamId} does not exist.");                
            }
            else if(team.SportId != sport!.Id)
            {
                ModelState.AddModelError("GameInvitations", $"Error! Team '{team.Name}' does not play the sport '{sport!.Name}'.");                
            }            
        }

        // Alternative Flow 4: If the user is not the captain of at least one of the selected teams, the system shows an error message because 
        // the user has no permission to perform this action
        if(!teams.Any(t => t.CaptainId == user!.Id)) // We can use the null-forgiving operator (!) here because we have already checked that the user is not null
        {
            ModelState.AddModelError("ResponsibleUserName", "Error! User is not the captain of any invited team.");
        }

        if(ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        // If all validations pass, we can create the game and save it to the database

        // Instance of Game entity with the data from the DTO. We set the ID to 0 because it will be generated by the database when we save it.
        var game = new Game(0, gameForCreate.Name, gameForCreate.Date, gameForCreate.Place, gameForCreate.Description, sport!, user!)
        {
            GameInvitations = new List<GameInvitation>()
        };
        
        // Add GameInvitations to the game entity
        foreach(var item in gameForCreate.GameInvitations)
        {
            game.GameInvitations.Add(new GameInvitation(item.TeamId, game.Id, item.Message)
            {
                AcceptedGame = false // POST-CONDITION: Mark the invitation as not "pending" by default 
            });
        }

        _context.Games.Add(game);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            ModelState.AddModelError("Game", "Error! There was an error saving your game, please, try again later.");
            return Conflict(new ValidationProblemDetails(ModelState));
        }


        // RETURN SUCCESS RESPONSE

        // Create a list of TeamForGameDetailDTO objects to include in the response
        var teamsDetail = gameForCreate.GameInvitations.Select(gi =>
        {
            var team = teams.First(t => t.Id == gi.TeamId);
            return new TeamForGameDetailDTO(
                team.Id,
                team.Name,
                team.Captain.Name,
                team.Captain.Surname,
                team.TeamInvitations.Count(ti => ti.InvitationAccepted),
                team.MinAge,
                team.MaxAge,
                team.Sport.Name,
                gi.Message
            );
        }).ToList();
        
        // Create a GameForDetailDTO object to RETURN in the response (Step 7)
        var gameDetail = new GameForDetailDTO(
            game.Id,
            game.Name,
            sport!.Name,
            game.Date,
            game.Place,
            game.Description,
            game.GameInvitations.Count,
            teamsDetail // List with the details of the invited teams in the response
        );

        return CreatedAtAction(nameof(GetGame), new {id = game.Id}, gameDetail); // Return the created game with a 201 Created status code
    }

}