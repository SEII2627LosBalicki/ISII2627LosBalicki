using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique =true)]
public class Team {

    // constructor w/o parameters
    public Team()
    {
        TeamInvitations = new List<TeamInvitation>();
    }

    // consturctor w/ parameters
    public Team(int id, string name, int maxMembers, int minAge, int maxAge, Sport sport, ApplicationUser captain) {
        Id = id;
        Name = name;
        MaxMembers = maxMembers;
        MinAge = minAge;
        MaxAge = maxAge;

        Sport = sport;
        SportId = sport?.Id ?? 0; //if sport = null, SportID=0
        Captain = captain;
        CaptainId = captain?.Id ?? string.Empty; //if captain = null, CaptainId is empty
        TeamInvitations = new List<TeamInvitation>();
    }

    public int Id { get; set; }

    [StringLength(9, MinimumLength = 4)]
    public string Name { get; set; } = "Team name";
    public int MaxMembers { get; set; }
    public int MinAge { get; set; } = 3;
    public int MaxAge { get; set; } = 3;

    // 1..N Sport
    public int SportId { get; set; }
    public Sport Sport { get; set; } = null!; //not null

    // 1..N ApplicationUser(Captain)
    public string CaptainId { get; set; } = string.Empty;
    public ApplicationUser Captain { get; set; } = null!;

    // N..N through TeamInvitation

    public IList<TeamInvitation> TeamInvitations { get; set; }
}