using AppForSEII.API.DTOs.RefereeAssignedToDTOs;
using AppForSEII.API.DTOs.RefereeGroupDTOs;
using AppForSEII.API.DTOs.RefereeDTOs;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RefereeGroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RefereeGroupsController> _logger;

    public RefereeGroupsController(ApplicationDbContext context, ILogger<RefereeGroupsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(RefereeGroupDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetRefereeGroup(int id)
    {
        var refereeGroup = await _context.RefereeGroups
            .Include(rg => rg.Game)
                .ThenInclude(g => g.Sport)
            .Include(rg => rg.RefereeAssignedTos)
                .ThenInclude(ra => ra.Referee)
            .Where(rg => rg.Id == id)
            .Select(rg => new RefereeGroupDTO(
                rg.Game.Name, rg.Game.Date, rg.Game.Place, rg.Game.Sport.Name,
                rg.Name, rg.Description, rg.Rules,
                rg.RefereeAssignedTos
                    .OrderBy(ra => ra.Referee.Surname)
                    .Select(ra => new RefereeAssignedToDTO(
                        ra.Referee.Name, ra.Referee.Surname, ra.Referee.YearsRefereeing,
                        ra.Role, ra.RoleDescription))
                    .ToList()))
            .FirstOrDefaultAsync();

        if (refereeGroup == null)
        {
            _logger.LogError($"Error: RefereeGroup with id {id} does not exist");
            return NotFound();
        }

        return Ok(refereeGroup);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(RefereeGroupDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> CreateRefereeGroup(RefereeGroupForCreateDTO refereeGroupForCreate)
    {
        var game = await _context.Games
            .Include(g => g.Sport)
            .FirstOrDefaultAsync(g => g.Id == refereeGroupForCreate.GameId);
        if (game == null)
        {
            ModelState.AddModelError("GameId", $"Game with id {refereeGroupForCreate.GameId} does not exist");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        if (await _context.RefereeGroups.AnyAsync(rg => rg.GameId == game.Id))
        {
            ModelState.AddModelError("GameId", "This game already has a referee group");
            return Conflict(new ValidationProblemDetails(ModelState));
        }

        if (await _context.RefereeGroups.AnyAsync(rg => rg.Name == refereeGroupForCreate.Name))
        {
            ModelState.AddModelError("Name", "A referee group with this name already exists");
            return Conflict(new ValidationProblemDetails(ModelState));
        }

        if (refereeGroupForCreate.Referees.Count < game.Sport.NumberofReferees)
        {
            ModelState.AddModelError("Referees",
                $"The sport {game.Sport.Name} needs at least {game.Sport.NumberofReferees} referees");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var refereeIds = refereeGroupForCreate.Referees.Select(r => r.RefereeId).ToList();
        if (refereeIds.Distinct().Count() != refereeIds.Count)
        {
            ModelState.AddModelError("Referees", "A referee cannot be assigned more than once");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        int validReferees = await _context.Referees
            .CountAsync(r => refereeIds.Contains(r.Id) && r.SportId == game.SportId);
        if (validReferees != refereeIds.Count)
        {
            ModelState.AddModelError("Referees", "Some referees do not exist or are not of the sport of the game");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var refereeGroup = new RefereeGroup
        {
            Name = refereeGroupForCreate.Name,
            Description = refereeGroupForCreate.Description,
            Rules = refereeGroupForCreate.Rules,
            GameId = game.Id,
            RefereeAssignedTos = refereeGroupForCreate.Referees
                .Select(r => new RefereeAssignedTo
                {
                    RefereeId = r.RefereeId,
                    Role = r.Role,
                    RoleDescription = r.RoleDescription
                })
                .ToList()
        };

        _context.RefereeGroups.Add(refereeGroup);
        await _context.SaveChangesAsync();


        var referees = await _context.Referees
            .Where(r => refereeIds.Contains(r.Id))
            .ToListAsync();

        var assignedDTOs = refereeGroupForCreate.Referees
            .Select(r =>
            {
                var referee = referees.First(x => x.Id == r.RefereeId);
                return new RefereeAssignedToDTO(referee.Name, referee.Surname,
                    referee.YearsRefereeing, r.Role, r.RoleDescription);
            })
            .OrderBy(a => a.Surname)
            .ToList();

        var refereeGroupDTO = new RefereeGroupDTO(game.Name, game.Date, game.Place,
            game.Sport.Name, refereeGroup.Name, refereeGroup.Description,
            refereeGroup.Rules, assignedDTOs);

        return CreatedAtAction("GetRefereeGroup", new { id = refereeGroup.Id }, refereeGroupDTO);
    }
    
}