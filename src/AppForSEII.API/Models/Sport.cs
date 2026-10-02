namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class Sport
{
    public Sport(int id, string name, string description, string basicRules)
    {
        Id = id;
        Name = name;
        MinimumNumberofPlayers = 2;
        NumberofReferees = 1;
        Description = description;
        BasicRules = basicRules;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public int MinimumNumberofPlayers { get; set; }
    public int NumberofReferees { get; set; }
    public string Description { get; set; }
    public string BasicRules { get; set; }

    public IList<Game> Games { get; set; } = new List<Game>();// Navigation property to Game
    
    public IList<Team> Teams { get; set; } = new List<Team>();
}   