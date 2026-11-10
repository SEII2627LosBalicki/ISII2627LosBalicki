using AppForSEII.API.Controllers;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Mvc;
using AppForSEII.UT;
using Moq;
using Xunit;
using AppForSEII.API.DTOs.ApplicationUserDTO;


namespace AppForSEII.UT.ApplicationUsersController_test;

public class GetUsersForTeam_test : AppForSEII4SqliteUT
{
    public GetUsersForTeam_test()
    {

        var sports = new List<Sport>()
        {
            new Sport(0, "Football", "Team sport playing 11vs11", "Don't touch the ball with the hands, except the goalkeeper."),
            new Sport(0, "Chess", "Board game", "Each player moves one piece per turn.")
        };


        var users = new List<ApplicationUser>()
        {
            new ApplicationUser("0", "Kryzstof", "Balicki", "balicki@gmail.com",DateOnly.FromDateTime(DateTime.Today).AddYears(-20), Gender.Male),
            new ApplicationUser("1", "Isi", "Palazón", "tucalvitofavorito@gmail.com", DateOnly.FromDateTime(DateTime.Today).AddYears(-30), Gender.Male),
            new ApplicationUser("2", "Carlos", "Kirk", "carloskirk@gmail.com", DateOnly.FromDateTime(DateTime.Today).AddYears(-40), Gender.Male),

        };



        var interests = new List<InterestedIn>()
        {
            new InterestedIn(sports[0], users[0], 4),   // Kryzstof: Football, skill 4
            new InterestedIn(sports[0], users[1], 3),   // Isi: Football, skill 3
            new InterestedIn(sports[1], users[1], 5),   // Isi: Chess, skill 5
            new InterestedIn(sports[1], users[2], 2),   // Carlos: Chess, skill 2
        };


        _context.AddRange(interests);
        _context.AddRange(sports);
        _context.AddRange(users);

        _context.SaveChanges();
    }


    public static IEnumerable<object?[]> TestCasesFor_GetUsersForTeam_OK()
    {

        //DTO por cada usuario
        var kryzstof = new UserForTeamDTO("0", "Kryzstof", "Balicki", 20, new List<SportSkillDTO>
        {
            new SportSkillDTO("Football", 4)
        });

        var isi = new UserForTeamDTO("1", "Isi", "Palazón", 30, new List<SportSkillDTO>
        {
            new SportSkillDTO("Football", 3),
            new SportSkillDTO("Chess", 5)
        });

        var carlos = new UserForTeamDTO("2", "Carlos", "Kirk", 40, new List<SportSkillDTO>
        {
            new SportSkillDTO("Chess", 2)
        });


        //Lista esperada de cada caso

        var todos = new List<UserForTeamDTO> {kryzstof, carlos, isi};
        var football = new List<UserForTeamDTO> {kryzstof, isi};
        var noKryzstof = new List<UserForTeamDTO> {carlos, isi};
        var kryzstofEIsi = new List<UserForTeamDTO> {kryzstof, isi};
        var soloIsi = new List<UserForTeamDTO> {isi};
        var soloCarlos = new List<UserForTeamDTO> {carlos};
        var ninguno = new List<UserForTeamDTO>();

        //Matriz con todas las filas: {sportName, minAge, maxAge, listaEsperada}
        return new List<object?[]>
        {
            new object?[] {null, null, null, todos, null},
            new object?[] {"Football", null, null, football, null},
            new object?[] {null, 25, null, noKryzstof, null},
            new object?[] {null, null, 35, kryzstofEIsi, null},
            new object?[] {null, 25, 35, soloIsi, null},
            new object?[] {"Football", 25, null, soloIsi, null},
            new object?[] {"Chess", null, 25, ninguno, null},
            new object?[] {"Chess", 35, 45, soloCarlos, null},
            new object?[] {null, null, null, soloCarlos, "Kir"},
            new object?[] {null, null, null, ninguno, "xyz"}
        };
    }


    [Theory]
    [MemberData(nameof(TestCasesFor_GetUsersForTeam_OK))]
    [Trait("Database", "WithoutFixture")]
    [Trait("LevelTesting", "Unit Testing")]
    public async Task GetUsersForTeam_OK_test(string? sportName, int? minAge, int? maxAge, IList<UserForTeamDTO> expectedUsers, string? surname)
    {
        // Arrange
        var mock = new Mock<ILogger<ApplicationUsersController>>();
        ILogger<ApplicationUsersController> logger = mock.Object;
        var controller = new ApplicationUsersController(_context, logger);

        // Act
        var result = await controller.GetUsersForTeam(sportName, minAge, maxAge, surname);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var usersActual = Assert.IsType<List<UserForTeamDTO>>(okResult.Value);
        Assert.Equal(expectedUsers, usersActual);
    }
}
