using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.RefereeAssignedToDTOs;
using AppForSEII.API.DTOs.RefereeGroupDTOs;

namespace AppForSEII.UT.RefereeGroupsController_test;

public class GetRefereeGroup_test : AppForSEII4SqliteUT
{
    public GetRefereeGroup_test()
    {
        // Phase 1: sport, responsible user, referees and game
        var sport = new Sport(1, "Padel", "Racquet sport", "Basic rules");
        var responsible = new ApplicationUser("1", "Isi", "Palazon", "isi@test.com");

        _context.Sports.Add(sport);
        _context.ApplicationUsers.Add(responsible);
        _context.Referees.Add(new Referee("2", "Koldo", "García", "koldo@test.com",
            rating: 2, yearsRefereeing: 2) { SportId = 1 });
        _context.Referees.Add(new Referee("3", "Ana", "López", "ana@test.com",
            rating: 5, yearsRefereeing: 3) { SportId = 1 });
        _context.Games.Add(new Game(1, "Padel Match 1", new DateTime(2026, 11, 3, 12, 0, 0),
            "MasQuePadel", "Test game", sport, responsible));
        _context.SaveChanges();

        // Phase 2: the referee group (its Id is generated here)
        var group = new RefereeGroup("PadelGrp", 1, "Test group", "Test rules");
        _context.RefereeGroups.Add(group);
        _context.SaveChanges();

        // Phase 3: the assignments, which need the group's Id
        _context.RefereeAssignedTos.Add(new RefereeAssignedTo(group.Id, "2", "Main", "Main referee"));
        _context.RefereeAssignedTos.Add(new RefereeAssignedTo(group.Id, "3", "Assistant", "Assistant referee"));
        _context.SaveChanges();
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetRefereeGroup_Found_test()
    {
        // Arrange
        var expectedGroup = new RefereeGroupDTO("Padel Match 1", new DateTime(2026, 11, 3, 12, 0, 0),
            "MasQuePadel", "Padel", "PadelGrp", "Test group", "Test rules",
            new List<RefereeAssignedToDTO>
            {
                new RefereeAssignedToDTO("Koldo", "García", 2, "Main", "Main referee"),
                new RefereeAssignedToDTO("Ana", "López", 3, "Assistant", "Assistant referee")
            });

        var mock = new Mock<ILogger<RefereeGroupsController>>();
        ILogger<RefereeGroupsController> logger = mock.Object;
        var controller = new RefereeGroupsController(_context, logger);

        // Act
        var result = await controller.GetRefereeGroup(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualGroup = Assert.IsType<RefereeGroupDTO>(okResult.Value);
        Assert.Equal(expectedGroup, actualGroup);
    }

    [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetRefereeGroup_NotFound_test()
    {
        // Arrange
        var mock = new Mock<ILogger<RefereeGroupsController>>();
        ILogger<RefereeGroupsController> logger = mock.Object;
        var controller = new RefereeGroupsController(_context, logger);

        // Act
        var result = await controller.GetRefereeGroup(0);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}