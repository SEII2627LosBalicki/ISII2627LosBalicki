using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.RefereeDTOs;

namespace AppForSEII.UT.RefereesController_test;

public class GetRefereesForRefereeGroup_test : AppForSEII4SqliteUT
{
    public GetRefereesForRefereeGroup_test()
    {
        // Phase 1: sports
        _context.Sports.Add(new Sport(1, "Football", "Team sport", "Basic rules"));
        _context.Sports.Add(new Sport(2, "Padel", "Racquet sport", "Basic rules"));
        _context.SaveChanges();

        // Phase 2: referees (they need an existing sport)
        _context.Referees.Add(new Referee("10", "Koldo", "García", "koldo@test.com",
            rating: 2, yearsRefereeing: 2) { SportId = 2 });
        _context.Referees.Add(new Referee("11", "Ana", "López", "ana@test.com",
            rating: 5, yearsRefereeing: 3) { SportId = 1 });
        _context.Referees.Add(new Referee("12", "Carlos", "Mateo", "carlos@test.com",
            rating: 4, yearsRefereeing: 5) { SportId = 1 });
        _context.Referees.Add(new Referee("13", "Luis", "Pérez", "luis@test.com",
            rating: 3, yearsRefereeing: 0) { SportId = 1 });
        _context.SaveChanges();
    }

        public static IEnumerable<object?[]> TestCasesFor_GetRefereesForRefereeGroup()
    {
        var koldo = new RefereeForRefereeGroupDTO("10", "Koldo", "García", 2, 2, "Padel");
        var ana = new RefereeForRefereeGroupDTO("11", "Ana", "López", 3, 5, "Football");
        var carlos = new RefereeForRefereeGroupDTO("12", "Carlos", "Mateo", 5, 4, "Football");

        var allTests = new List<object?[]>
        {
            // sportName, yearsRefereeing, expected result
            new object?[] { null, null, new List<RefereeForRefereeGroupDTO> { koldo, ana, carlos } },
            new object?[] { "Football", null, new List<RefereeForRefereeGroupDTO> { ana, carlos } },
            new object?[] { null, 3, new List<RefereeForRefereeGroupDTO> { ana, carlos } },
            new object?[] { "Football", 4, new List<RefereeForRefereeGroupDTO> { carlos } },
            new object?[] { "Padel", 3, new List<RefereeForRefereeGroupDTO>() }
        };

        return allTests;
    }

    [Theory]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_GetRefereesForRefereeGroup))]
    public async Task GetRefereesForRefereeGroup_Filters_test(string? sportName,
        int? yearsRefereeing, List<RefereeForRefereeGroupDTO> expectedReferees)
    {
        // Arrange
        var mock = new Mock<ILogger<RefereesController>>();
        ILogger<RefereesController> logger = mock.Object;
        var controller = new RefereesController(_context, logger);

        // Act
        var result = await controller.GetRefereesForRefereeGroup(sportName, yearsRefereeing);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualReferees = Assert.IsType<List<RefereeForRefereeGroupDTO>>(okResult.Value);
        Assert.Equal(expectedReferees, actualReferees);
    }
}