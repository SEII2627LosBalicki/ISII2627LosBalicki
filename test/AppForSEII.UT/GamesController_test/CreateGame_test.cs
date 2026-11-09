using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.GameDTOs;
using AppForSEII.API.DTOs.TeamDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AppForSEII.UT.GamesController_test;

public class CreateGame_test : AppForSEII4SqliteUT
{
    private const string _sportFootball = "Football";
    private const string _sportPadel = "Padel";

    private const string _userTroy = "troy@test.com";
    private const string _userBalicki = "balicki@test.com";
    private const string _userCigala = "elcigala@test.com"; // No captain, just a user

    private const string _teamPierogiss = "Pierogiss";
    private const string _teamWildcats = "Wildcats";
    private const string _teamKirks = "Kirks";

    private const string _existingGameName = "Existing Game";
    private const string _gamePlace = "Carlos Belmonte";
    private const string _gameDescription = "The final match of the Hipertensiones League.";
    private const string _invitationMessage = "We would love to play against you in the final match!";

    public CreateGame_test()
    {
        // Sports
        var football = new Sport(0, _sportFootball, "Team sport playing 11vs11", "Don't touch the ball with the hands, except the goalkeeper.");
        var padel = new Sport(0, _sportPadel, "Team sport playing with racquets.", "The ball can bounce off the walls.");
        _context.Sports.AddRange(football, padel);

        // Users
        var userTroy = new ApplicationUser("1", "Troy", "Bolton", _userTroy);
        var userBalicki = new ApplicationUser("2", "Krzystof", "Balicki", _userBalicki);
        var userCigala = new ApplicationUser("3", "El", "Cigala", _userCigala);
        _context.ApplicationUsers.AddRange(userTroy, userBalicki, userCigala);

        // Teams
        var teamWildcats = new Team(0, _teamWildcats, null, 20, 18, 40, football, userTroy);
        var teamPierogiss = new Team(0, _teamPierogiss, null, 20, 18, 40, football, userBalicki);
        var teamKirks = new Team(0, _teamKirks, null, 15, 18, 40, padel, userTroy);

        var memberInvitation = new TeamInvitation(userCigala, teamWildcats, "Welcome to Wildcats", true);
        teamWildcats.TeamInvitations.Add(memberInvitation);

        _context.Teams.AddRange(teamWildcats, teamPierogiss, teamKirks);
        _context.TeamInvitations.Add(memberInvitation);

        // Existing Game (to test the error of creating a game with the same name)
        var existingGame = new Game(0, _existingGameName, DateTime.Today.AddDays(10), _gamePlace, _gameDescription, football, userTroy);
        _context.Games.Add(existingGame);

        _context.SaveChanges(); // Save changes to generate IDs for the entities        
    }

    public static IEnumerable<object[]> TestCasesFor_CreateGame_Error()
    {
        var futureDate = DateTime.Today.AddDays(10); // A date in the future for valid game creation

        var validInvitations = new List<GameInvitationDTO>
        {
            new GameInvitationDTO(1, _invitationMessage),
            new GameInvitationDTO(2, _invitationMessage)
        };

        // 1. Date in the past
        var gameDateInPast = new GameForCreateDTO("Game Date Past", DateTime.Now.AddDays(-1), _gamePlace, _gameDescription, _sportFootball, _userTroy, validInvitations);

        // 2. Game already exists in database
        var gameDuplicateName = new GameForCreateDTO(_existingGameName, futureDate, _gamePlace, _gameDescription, _sportFootball, _userTroy, validInvitations);

        // 3. Less than 2 teams invited (Alternative flow 3)
        var gameLessTwoTeams = new GameForCreateDTO("Game less than 2 teams", futureDate, _gamePlace, _gameDescription, _sportFootball, _userTroy, new List<GameInvitationDTO> {new GameInvitationDTO(1, _invitationMessage)});

        // 4. Responsible user not registered (Post-condition)
        var gameUnregisteredUser = new GameForCreateDTO("Game Unregistered User", futureDate, _gamePlace, _gameDescription, _sportFootball, "unregistered@test.com", validInvitations);

        // 5. Sport does not exist
        var gameSportNotFound = new GameForCreateDTO("Game Sport not found", futureDate, _gamePlace, _gameDescription, "Curling", _userTroy, validInvitations);

        // 6. Same team invited more than once (Complex validation)
        var duplicateInvitations = new List<GameInvitationDTO>
        {
            new GameInvitationDTO(1, _invitationMessage),
            new GameInvitationDTO(1, _invitationMessage)
        };

        var gameDuplicateTeamInvitations = new GameForCreateDTO("Game Duplicate Team", futureDate, _gamePlace, _gameDescription, _sportFootball, _userTroy, duplicateInvitations);

        // 7. Invited team does not exist (Complex validation)
        var nonExistentTeamInvitations = new List<GameInvitationDTO>
        {
            new GameInvitationDTO(1, _invitationMessage), 
            new GameInvitationDTO(99, _invitationMessage) // Team with ID 99 does not exist   
        };

        var gameTeamNotFound = new GameForCreateDTO ("Game Team Not Found", futureDate, _gamePlace, _gameDescription, _sportFootball, _userTroy, nonExistentTeamInvitations);

        // 8. Team does not play the sport of the game (Complex validation)
        var wrongSportInvitation = new List<GameInvitationDTO>
        {
            new GameInvitationDTO(1, _invitationMessage), // Wildcats (Football)
            new GameInvitationDTO(3, _invitationMessage)  // Kirks (Padel)
        };

        var gameTeamWrongSport = new GameForCreateDTO("Game Wrong Sport", futureDate, _gamePlace, _gameDescription, _sportFootball, _userTroy, wrongSportInvitation);

        // 9. User is not the captain of any invited team (El Cigala is not a captain of any team)
        var gameUserNotCaptain = new GameForCreateDTO("Game User Not Captain", futureDate, _gamePlace, _gameDescription, _sportFootball, _userCigala, validInvitations);

        var allTests = new List<object[]>
        {
            new object[] {gameDateInPast, "Error! Your game date is in the past."},
            new object[] {gameDuplicateName, "Error! Game with this name already exists."},
            new object[] {gameLessTwoTeams, "Error! You must invite at least 2 teams to the game."},
            new object[] {gameUnregisteredUser, "Error! Responsible user is not registered."},
            new object[] {gameSportNotFound, "Error! Sport does not exist."},
            new object[] {gameDuplicateTeamInvitations, "Error! A team cannot be invited more than once."},
            new object[] {gameTeamNotFound, "Error! Team with ID 99 does not exist."},
            new object[] {gameTeamWrongSport, "Error! Team 'Kirks' does not play the sport 'Football'."},
            new object[] {gameUserNotCaptain, "Error! User is not the captain of any invited team."}
        };

        return allTests;
    }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        [MemberData(nameof(TestCasesFor_CreateGame_Error))]
        public async Task CreateGame_Error_Test(GameForCreateDTO gameDTO, string errorExpected)
        {
            // Arrange
            var mock = new Mock<ILogger<GamesController>>();
            ILogger<GamesController> logger = mock.Object;
            var controller = new GamesController(logger, _context);

            // Act
            var result = await controller.CreateGame(gameDTO);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var problemDetails = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            var errorActual = problemDetails.Errors.First().Value[0];

            Assert.StartsWith(errorExpected, errorActual); // Check that the actual error message starts with the expected error message

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task CreateGame_Success_Test()
        {
            // Arrange
            var mock = new Mock<ILogger<GamesController>>();
            ILogger<GamesController> logger = mock.Object;
            var controller = new GamesController(logger, _context);

            var futureDate = DateTime.Today.AddDays(10); // A date in the future for valid game creation

            var invitations = new List<GameInvitationDTO>
            {
                new GameInvitationDTO(1, _invitationMessage),
                new GameInvitationDTO(2, _invitationMessage)
            };

            var gameForCreateDTO = new GameForCreateDTO("Final championship", futureDate, _gamePlace, _gameDescription, _sportFootball, _userTroy, invitations);

            var expectedTeamDetails = new List<TeamForGameDetailDTO>
            {
                new TeamForGameDetailDTO(1, _teamWildcats, "Troy", "Bolton", 1, 18, 40, _sportFootball, _invitationMessage),
                new TeamForGameDetailDTO(2, _teamPierogiss, "Krzystof", "Balicki", 0, 18, 40, _sportFootball, _invitationMessage)
            };

            var expectedGameDetailDTO = new GameForDetailDTO(2, "Final championship", _sportFootball, futureDate, _gamePlace, _gameDescription, 2, expectedTeamDetails);

            // Act
            var result = await controller.CreateGame(gameForCreateDTO);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualGameDetailDTO = Assert.IsType<GameForDetailDTO>(createdResult.Value);

            Assert.Equal(expectedGameDetailDTO, actualGameDetailDTO); // Check that the actual game detail matches the expected game detail
        }


    


}