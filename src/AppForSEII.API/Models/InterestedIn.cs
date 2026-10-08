using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(SportId), nameof(UserId))]

public class InterestedIn
{
    public InterestedIn()
    { 
    }

    public InterestedIn(Sport sport, ApplicationUser user, int skill = 5)
    {
        Sport = sport;
        SportId = sport?.Id ?? 0;
        User = user;
        UserId = user?.Id ?? "0";
        Skill = skill;
    }

    public int SportId { get; set; }
    public Sport Sport { get; set; } = null!;
    public string UserId { get; set; } = "0";
    public ApplicationUser User { get; set; } = null!;

    [Range(1, 5)]
    public int Skill { get; set; } = 5;
}