using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.GameDTOs;
using AppForSEII.API.DTOs.TeamDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.GamesController_test;

public class GetGame_test : AppForSEII4SqliteUT
{
    public GetGame_test()
    {
        // Sport, responsible and captain for the game and team to be created
        var sport = new Sport(0, "Football", "Team sport playing 11vs11", "Don't touch the ball with the hands, except the goalkeeper.");
        var responsible = new ApplicationUser("1", "Hernán", "Cortés", "conqueror4000@gmail.com");
        var captain = new ApplicationUser("2", "Kryzstof", "Balicki", "balicki@gmail.com");

        _context.Sports.Add(sport); // Add the sport to the context
        _context.ApplicationUsers.AddRange(responsible, captain); // Add the responsible and captain to the context

        // Team invited
        var team = new Team(0, "Pierogiss", null, 20, 18, 40, sport, captain);
        _context.Teams.Add(team); // Add the team to the context

        // Game
        var game = new Game(0, "Liga Hypermotion Final", new DateTime(2026, 12, 1, 18, 0, 0), "Carlos Belmonte", "The final match of the Hipertensiones League.", sport, responsible);
        _context.Games.Add(game); // Add the game to the context

        _context.SaveChanges(); // Save changes to generate IDs for the entities, because the GameInvitation needs the IDs of the Game and Team to be created

        // Game invitation
        var gameInvitation = new GameInvitation(team.Id, game.Id, "We would love to play against you in the final match!")
        {
            TeamInvited = team,
            GameInvited = game,
            AcceptedGame = true
        };
        _context.GameInvitations.Add(gameInvitation); // Add the game invitation to the context
        _context.SaveChanges(); // Save changes to generate IDs for the game invitation
    }

    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]    
    public async Task GetGame_NotFound_test()
    {
        // Arrange
        var mock = new Mock<ILogger<GamesController>>(); // As we don't want to write logs to the console during testing, we use Moq to create a mock logger.
        ILogger<GamesController> logger = mock.Object; 
        var controller = new GamesController(logger, _context); // Instantiate the GamesController with the mock logger and the context

        // Act
        var result = await controller.GetGame(0); // Game with id 0 does not exist

        // Assert
        Assert.IsType<NotFoundResult>(result); // Check that the result is of type NotFoundResult
    }

    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]    
    public async Task GetGame_Found_test()
    {
        // Arrange
        var mock = new Mock<ILogger<GamesController>>();
        ILogger<GamesController> logger = mock.Object;
        var controller = new GamesController(logger, _context);

        var expectedTeams = new List<TeamForGameDetailDTO>
        {
            new TeamForGameDetailDTO(1, "Pierogiss", "Kryzstof", "Balicki", 0, 18, 40, "Football", "We would love to play against you in the final match!")
        };

        var expectedGame = new GameForDetailDTO(1, "Liga Hypermotion Final", "Football", new DateTime(2026, 12, 1, 18, 0, 0), "Carlos Belmonte", "The final match of the Hipertensiones League.", 1, expectedTeams);

        // Act
        var result = await controller.GetGame(1);

        // Assert
        // Check that the response type is OK and obtain the Game
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualGame = Assert.IsType<GameForDetailDTO>(okResult.Value);

        // Check that the actual game matches the expected game
        Assert.Equal(expectedGame, actualGame);
        
    }


}
