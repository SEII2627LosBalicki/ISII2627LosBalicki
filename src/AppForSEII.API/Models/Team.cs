using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.Models;

public class Team {
    public Team() {}

    public Team(int id, string name, int maxMembers, int minAge, int maxAge, string? description = "Team name") {
        Id = id;
        Name = name;
        MaxMembers = maxMembers;
        MinAge = minAge;
        MaxAge = maxAge;
        Description = description;
    }

    public int Id { get; set; }

    [StringLength(9)]
    public string Name { get; set; } = "Team name";
    [StringLength(9)]
    public string? Description { get; set; } = "Team name";
    public int MaxMembers { get; set; }
    public int MinAge { get; set; } = 3;
    public int MaxAge { get; set; } = 3;
}