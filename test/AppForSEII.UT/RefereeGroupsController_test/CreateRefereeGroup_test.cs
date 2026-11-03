using AppForSEII.API.Controllers;
using AppForSEII.API.DTOs.RefereeAssignedToDTOs;
using AppForSEII.API.DTOs.RefereeDTOs;
using AppForSEII.API.DTOs.RefereeGroupDTOs;

namespace AppForSEII.UT.RefereeGroupsController_test;

public class CreateRefereeGroup_test : AppForSEII4SqliteUT
{
    public CreateRefereeGroup_test()
    {
        // Phase 1: sports, responsible user, referees and games
        var padel = new Sport(1, "Padel", "Racquet sport", "Basic rules") { NumberofReferees = 2 };
        var football = new Sport(2, "Football", "Team sport", "Basic rules");
        var responsible = new ApplicationUser("1", "Isi", "Palazon", "isi@test.com");

        _context.Sports.Add(padel);
        _context.Sports.Add(football);
        _context.ApplicationUsers.Add(responsible);

        _context.Referees.Add(new Referee("10", "Koldo", "García", "koldo@test.com",
            rating: 2, yearsRefereeing: 2) { SportId = 1 });
        _context.Referees.Add(new Referee("11", "Ana", "López", "ana@test.com",
            rating: 5, yearsRefereeing: 3) { SportId = 1 });
        _context.Referees.Add(new Referee("12", "Carlos", "Mateo", "carlos@test.com",
            rating: 4, yearsRefereeing: 5) { SportId = 2 });

        // Game 1: free (no referee group yet). Game 2: already has a group.
        _context.Games.Add(new Game(1, "Padel Match 1", new DateTime(2026, 11, 10, 18, 0, 0),
            "MasQuePadel", "Free game", padel, responsible));
        _context.Games.Add(new Game(2, "Padel Match 2", new DateTime(2026, 11, 11, 18, 0, 0),
            "MasQuePadel", "Game with group", padel, responsible));
        _context.SaveChanges();

        // Phase 2: an existing referee group for game 2
        _context.RefereeGroups.Add(new RefereeGroup("ExistGrp", 2, "Existing group", "Existing rules"));
        _context.SaveChanges();
    }

        public static IEnumerable<object[]> TestCasesFor_CreateRefereeGroup_BadRequest()
    {
        var koldo = new RefereeAssignedToForCreateDTO("10", "Main", "Main referee");
        var ana = new RefereeAssignedToForCreateDTO("11", "Assistant", "Assistant referee");
        var carlos = new RefereeAssignedToForCreateDTO("12", "Assistant", "Assistant referee");

        var allTests = new List<object[]>
        {
            // 1. The game does not exist
            new object[] {
                new RefereeGroupForCreateDTO(99, "NewGrp", "desc", "rules",
                    new List<RefereeAssignedToForCreateDTO> { koldo, ana }),
                "Game with id 99 does not exist" },

            // 2. Not enough referees (Padel needs 2)
            new object[] {
                new RefereeGroupForCreateDTO(1, "NewGrp", "desc", "rules",
                    new List<RefereeAssignedToForCreateDTO> { koldo }),
                "The sport Padel needs at least 2 referees" },

            // 3. The same referee twice
            new object[] {
                new RefereeGroupForCreateDTO(1, "NewGrp", "desc", "rules",
                    new List<RefereeAssignedToForCreateDTO> { koldo, koldo }),
                "A referee cannot be assigned more than once" },

            // 4. A referee of another sport (Carlos is Football)
            new object[] {
                new RefereeGroupForCreateDTO(1, "NewGrp", "desc", "rules",
                    new List<RefereeAssignedToForCreateDTO> { koldo, carlos }),
                "Some referees do not exist or are not of the sport of the game" }
        };

        return allTests;
    }

        [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_CreateRefereeGroup_BadRequest))]
    public async Task CreateRefereeGroup_BadRequest_test(
        RefereeGroupForCreateDTO refereeGroupForCreate, string errorExpected)
    {
        // Arrange
        var mock = new Mock<ILogger<RefereeGroupsController>>();
        ILogger<RefereeGroupsController> logger = mock.Object;
        var controller = new RefereeGroupsController(_context, logger);

        // Act
        var result = await controller.CreateRefereeGroup(refereeGroupForCreate);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);
        Assert.Equal(errorExpected, problemDetails.Errors.First().Value[0]);
    }

        public static IEnumerable<object[]> TestCasesFor_CreateRefereeGroup_Conflict()
    {
        var koldo = new RefereeAssignedToForCreateDTO("10", "Main", "Main referee");
        var ana = new RefereeAssignedToForCreateDTO("11", "Assistant", "Assistant referee");

        var allTests = new List<object[]>
        {
            // 5. The game already has a referee group (game 2)
            new object[] {
                new RefereeGroupForCreateDTO(2, "NewGrp", "desc", "rules",
                    new List<RefereeAssignedToForCreateDTO> { koldo, ana }),
                "This game already has a referee group" },

            // 6. The group name is already used ("ExistGrp"), on a free game (game 1)
            new object[] {
                new RefereeGroupForCreateDTO(1, "ExistGrp", "desc", "rules",
                    new List<RefereeAssignedToForCreateDTO> { koldo, ana }),
                "A referee group with this name already exists" }
        };

        return allTests;
    }

        [Theory]
    [Trait("LevelTesting", "Unit Testing")]
    [MemberData(nameof(TestCasesFor_CreateRefereeGroup_Conflict))]
    public async Task CreateRefereeGroup_Conflict_test(
        RefereeGroupForCreateDTO refereeGroupForCreate, string errorExpected)
    {
        // Arrange
        var mock = new Mock<ILogger<RefereeGroupsController>>();
        ILogger<RefereeGroupsController> logger = mock.Object;
        var controller = new RefereeGroupsController(_context, logger);

        // Act
        var result = await controller.CreateRefereeGroup(refereeGroupForCreate);

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        var problemDetails = Assert.IsType<ValidationProblemDetails>(conflictResult.Value);
        Assert.Equal(errorExpected, problemDetails.Errors.First().Value[0]);
    }

        [Fact]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateRefereeGroup_Success_test()
    {
        // Arrange
        var refereeGroupForCreate = new RefereeGroupForCreateDTO(1, "NewGrp", "Test group", "Test rules",
            new List<RefereeAssignedToForCreateDTO>
            {
                new RefereeAssignedToForCreateDTO("10", "Main", "Main referee"),
                new RefereeAssignedToForCreateDTO("11", "Assistant", "Assistant referee")
            });

        // The response is ordered by surname: García (Koldo), López (Ana)
        var expectedGroup = new RefereeGroupDTO("Padel Match 1", new DateTime(2026, 11, 10, 18, 0, 0),
            "MasQuePadel", "Padel", "NewGrp", "Test group", "Test rules",
            new List<RefereeAssignedToDTO>
            {
                new RefereeAssignedToDTO("Koldo", "García", 2, "Main", "Main referee"),
                new RefereeAssignedToDTO("Ana", "López", 3, "Assistant", "Assistant referee")
            });

        var mock = new Mock<ILogger<RefereeGroupsController>>();
        ILogger<RefereeGroupsController> logger = mock.Object;
        var controller = new RefereeGroupsController(_context, logger);

        // Act
        var result = await controller.CreateRefereeGroup(refereeGroupForCreate);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetRefereeGroup", createdResult.ActionName);
        var actualGroup = Assert.IsType<RefereeGroupDTO>(createdResult.Value);
        Assert.Equal(expectedGroup, actualGroup);

        // The group really is in the database, with pending (not accepted) assignments
        using var freshContext = CreateContext();
        var savedGroup = freshContext.RefereeGroups
            .Include(rg => rg.RefereeAssignedTos)
            .Single(rg => rg.Name == "NewGrp");
        Assert.Equal(2, savedGroup.RefereeAssignedTos.Count);
        Assert.All(savedGroup.RefereeAssignedTos, ra => Assert.False(ra.AcceptedAssignment));
    }
}