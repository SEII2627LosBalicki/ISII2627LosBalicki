using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.TeamDTOs;
using Microsoft.AspNetCore.Mvc;
using AppForSEII.UT;
using Moq;
using Xunit;
using System.Reflection;


namespace AppForSEII.UT.TeamsController_test;

public class CreateTeam_test : AppForSEII4SqliteUT
{
    public CreateTeam_test()
    {
        var sports = new List<Sport>()
        {
            new Sport(0, "Football", "Team sport playing 11vs11", "Don't touch the ball with the hands, except the goalkeeper."),
            new Sport(0, "Chess", "Board game", "Each player moves one piece per turn.")
        };
        // Chess accepts a team of one
        sports[1].MinimumNumberofPlayers = 1;

        var users = new List<ApplicationUser>()
        {
            new ApplicationUser("0", "Kryzstof", "Balicki", "balicki@gmail.com", DateOnly.Parse("1985-01-01"), Gender.Male),
            new ApplicationUser("1", "Isi", "Palazón", "isi@gmail.com", DateOnly.Parse("1999-01-01"), Gender.Male),
            new ApplicationUser("2", "Carlos", "Kirk", "carloskirk@gmail.com", DateOnly.Parse("1990-01-01"), Gender.Male),
            new ApplicationUser("3", "Ana", "López", "ana@gmail.com", DateOnly.Parse("2000-01-01"), Gender.Female),
            new ApplicationUser("4", "Marta", "Ruiz", "marta@gmail.com", DateOnly.Parse("1995-01-01"), Gender.Female),
        };

        var teams = new List<Team>()
        {
            new Team(0, "Kirks", null, 20, 18, 40, sports[0], users[2]),
        };

        _context.AddRange(sports);
        _context.AddRange(users);
        _context.AddRange(teams);
        _context.SaveChanges();
    }

    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_InvalidModel_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        controller.ModelState.AddModelError("clave", "Invalid Model");
        //Act
        var result = await controller.CreateTeam(new TeamForCreateDTO());

        //Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid Model", badResult.Value);

    }


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_MinAgeGreaterThanMaxAge_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {MinAge =  50, MaxAge = 25};
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("minAge can't be greater than maxAge", badResult.Value);

    }


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_RepeatedTeamName_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {Name = "Kirks"};
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Name can not be repeated", badResult.Value);

    }


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_NonExistingSport_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO { Name = "Nuevo", SportName = "sdbvdsojkbve"};
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("There is no sport with this name", notFound.Value);

    }


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_NonExistingCaptain_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {SportName = "Football", CaptainId = "99"};
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("There is no captain with this Id", notFound.Value);

    }


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_MinimumPlayers_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {SportName = "Football", CaptainId = "0"};
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Less members than the MinimumNumberOfPlayers (2). Select more players", badRequest.Value);

    }

    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_CaptainInInvited_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {SportName = "Football", CaptainId = "0", InvitedPlayers = new List<TeamInvitationDTO> 
            {
                new TeamInvitationDTO {Id = "0", InvitationMessage = "Hola"}
            }
        };
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The captain can't be in the list of invited players", badRequest.Value);

    }



    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_RepeatedInvitedPlayer_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {SportName = "Football", CaptainId = "0", InvitedPlayers = new List<TeamInvitationDTO> 
            {
                new TeamInvitationDTO {Id = "1", InvitationMessage = "Hola"},
                new TeamInvitationDTO {Id = "1", InvitationMessage = "Hola"}
            }
        };
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The user 1 is invited more than once", badRequest.Value);

    }


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_NotExistingInvitedPlayer_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {SportName = "Football", CaptainId = "0", InvitedPlayers = new List<TeamInvitationDTO> 
            {
                new TeamInvitationDTO {Id = "199", InvitationMessage = "Hola"}
            }
        };
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("There is no user with Id 199", notFound.Value);

    }

    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_TooManyPlayers_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {SportName = "Football", CaptainId = "0", MaxMembers = 1, InvitedPlayers = new List<TeamInvitationDTO> 
            {
                new TeamInvitationDTO {Id = "3", InvitationMessage = "Hola"}
            }
        };
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Captain plus invited players exceed MaxMembers (1)", badRequest.Value);

    }


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task CreateTeam_SuccessInvited_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        var dto = new TeamForCreateDTO {Name = "Vodka Juniors", SportName = "Football", CaptainId = "0", MaxMembers = 10, InvitedPlayers = new List<TeamInvitationDTO> 
            {
                new TeamInvitationDTO {Id = "3", InvitationMessage = "Hola"}
            }
        };
        //Act
        var result = await controller.CreateTeam(dto);

        //Assert
        var createdAtAction = Assert.IsType<CreatedAtActionResult>(result);
        var actual = Assert.IsType<TeamDetailDTO>(createdAtAction.Value);

        var captain = _context.ApplicationUsers.Find("0")!;
        var invited = _context.ApplicationUsers.Find("3")!;

        var expected = new TeamDetailDTO(2, "Vodka Juniors", "Kryzstof", "Balicki", "Football", 3, 3, captain.Age,
            new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("Ana", "López", "3", invited.Age, "Hola")
            });

        Assert.Equal(expected, actual);


    }


    
}