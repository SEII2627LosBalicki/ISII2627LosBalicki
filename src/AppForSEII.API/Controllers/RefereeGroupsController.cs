using AppForSEII.API.DTOs.RefereeAssignedToDTOs;
using AppForSEII.API.DTOs.RefereeGroupDTOs;

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
}