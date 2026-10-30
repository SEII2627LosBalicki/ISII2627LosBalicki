using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.TeamDTOs;
using Microsoft.AspNetCore.Mvc;
using AppForSEII.UT;
using Moq;
using Xunit;


namespace AppForSEII.UT.TeamsController_test;

public class GetTeamsForGame_test : AppForSEII4SqliteUT
{
    public GetTeamsForGame_test()
    {        
        var sports = new List<Sport>()
        {
            new Sport (0, "Football", "Team sport playing 11vs11", "Don't touch the ball with the hands, except the goalkeeper."),
            new Sport (0, "Padel", "Team sport playing with racquets.", "The ball can bounce off the walls."),
        };

        var users = new List<ApplicationUser>()
        {
            new ApplicationUser("0", "Kryzstof", "Balicki", "balicki@gmail.com", DateOnly.Parse("1985-01-01"), Gender.Male),
            new ApplicationUser("1", "Isi", "Palazón", "tucalvitofavorito@gmail.com", DateOnly.Parse("1999-01-01"), Gender.Male),
            new ApplicationUser("2", "Carlos", "Kirk", "carloskirk@gmail.com", DateOnly.Parse("1990-01-01"), Gender.Male),
            
        };

        var teams = new List<Team>()
        {
            new Team(0, "Pierogiss", null, 20, 18, 40, sports[0], users[0]),
            new Team(0, "Melaniers", null, 15, 16, 50, sports[0], users[1]),
            new Team(0, "Kirks", null, 13, 12, 30, sports[1], users[2]),
        };

        _context.AddRange(sports);
        _context.AddRange(users);
        _context.AddRange(teams);
        _context.SaveChanges();
    }

    public static IEnumerable<object?[]> TestCasesFor_GetTeamsForGame_OK()
    {
        // DTO's expected to be returned by the controller action GetTeamsForGame
        var teamDTOs = new List<TeamForGameDTO>()
        {
            new TeamForGameDTO(1, "Pierogiss", "Kryzstof", "Balicki", 0, 18, 40, "Football"),
            new TeamForGameDTO(2, "Melaniers", "Isi", "Palazón", 0, 16, 50, "Football"),
            new TeamForGameDTO(3, "Kirks", "Carlos", "Kirk", 0, 12, 30, "Padel")
        };

        // Test Case 1: No filters applied, all teams should be returned ordered by name
        var teamsTC1 = new List<TeamForGameDTO>() {teamDTOs[0], teamDTOs[1], teamDTOs[2]}
            .OrderBy(t => t.Name).ToList();

        // Test Case 2: Filter by captainName "Carlos", only the team with captain "Carlos"(Kirks) should be returned
        var teamsTC2 = new List<TeamForGameDTO>() {teamDTOs[2]};

        // Test Case 3: Filter by sport "Football", only the teams with sport "Football"(Pierogiss, Melaniers) should be returned ordered by name
        var teamsTC3 = new List<TeamForGameDTO>() {teamDTOs[0], teamDTOs[1]}
            .OrderBy(t => t.Name).ToList();

        // Test Case 4: Both filters -> return Kirks team
        var teamsTC4 = new List<TeamForGameDTO>() {teamDTOs[2]};

        // Test Matrix: each element is a new object?[] {captainName, sportName, expectedResult}
        var allTests = new List<object?[]>
        {
            new object?[] {null, null, teamsTC1},
            new object?[] {"Carlos", null, teamsTC2},
            new object?[] {null, "Football", teamsTC3},
            new object?[] {"Carlos", "Padel", teamsTC4},
        };

        return allTests;    
    }

    [Theory]
    [MemberData(nameof(TestCasesFor_GetTeamsForGame_OK))]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetTeamsForGame_OK_test(string? filterCaptainName, string? filterSportName, IList<TeamForGameDTO> expectedTeams)
    {
        // Arrange (Prepare the mock logger and the controller)
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        // Act
        var result = await controller.GetTeamsForGame(filterCaptainName, filterSportName);

        // Assert
        // Check that the response type is OK
        var okResult = Assert.IsType<OkObjectResult>(result);
        var teamDTOsActual = Assert.IsType<List<TeamForGameDTO>>(okResult.Value);
        Assert.Equal(expectedTeams, teamDTOsActual);
    }

}