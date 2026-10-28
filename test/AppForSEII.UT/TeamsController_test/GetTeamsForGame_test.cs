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
            new Sport (1, "Padel", "Team sport playing with racquets.", "The ball can bounce off the walls."),
        };

        var users = new List<ApplicationUser>()
        {
            new ApplicationUser("0", "Kryzstof", "Balicki", "balicki@gmail.com", DateOnly.Parse("1985-01-01"), Gender.Male),
            new ApplicationUser("1", "Isi", "Palazón", "tucalvitofavorito@gmail.com", DateOnly.Parse("1999-01-01"), Gender.Male),
            new ApplicationUser("2", "Carlos", "Kirk", "carloskirk@gmail.com", DateOnly.Parse("1990-01-01"), Gender.Male),
            
        };

        var teams = new List<Team>()
        {
            new Team(0, "Pierogisss", null, 20, 18, 40, sports[0], users[0]),
            new Team(1, "Melaniers", null, 15, 16, 50, sports[0], users[1]),
            new Team(2, "Kirks", null, 13, 12, 30, sports[1], users[2]),
        };
    }
}