using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.TeamDTOs;
using Microsoft.AspNetCore.Mvc;
using AppForSEII.UT;
using Moq;
using Xunit;
using Microsoft.VisualBasic;


namespace AppForSEII.UT.TeamsController_test;

public class GetTeam_test : AppForSEII4SqliteUT
{
    public GetTeam_test()
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

        var invitations = new List<TeamInvitation>()
        {
            new TeamInvitation(users[2], teams[0], "Hola")
        };

        _context.AddRange(invitations);
        _context.AddRange(sports);
        _context.AddRange(users);
        _context.AddRange(teams);
        _context.SaveChanges();
    }



    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetTeam_OK_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        // Act
        var result = await controller.GetTeam(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actual = Assert.IsType<TeamDetailDTO>(okResult.Value);

        var captain = _context.ApplicationUsers.Find("0")!;
        var edad = _context.ApplicationUsers.Find("2")!;

        var expected = new TeamDetailDTO(1, "Pierogiss", "Kryzstof", "Balicki", "Football", 18, 40, captain.Age,
            new List<TeamInvitationDTO>
            {
                new TeamInvitationDTO("Carlos", "Kirk", "2", edad.Age, "Hola")
            });        

        Assert.Equal(expected, actual);
    }

    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetTeam_NotExistingTeam_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        //Act
        var result = await controller.GetTeam(99);

        //Assert
        Assert.IsType<NotFoundResult>(result);
    }    


    [Fact]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetTeam_NotNullOrNegative_test()
    {
        //Arrange
        var mock = new Mock<ILogger<TeamsController>>();
        ILogger<TeamsController> logger = mock.Object;
        var controller = new TeamsController(logger, _context);

        //Act
        var result = await controller.GetTeam(-1);

        //Assert
        Assert.IsType<NotFoundResult>(result);
    }

}