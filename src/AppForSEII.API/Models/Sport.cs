namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class Sport
{
    public Sport(int id, string name, int minimumNumberofPlayers, int numberofReferees, string description, string basicRules)
    {
        Id = id;
        Name = name;
        MinimumNumberofPlayers = minimumNumberofPlayers;
        NumberofReferees = numberofReferees;
        Description = description;
        BasicRules = basicRules;
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinimumNumberofPlayers { get; set; } = 2;
    public int NumberofReferees { get; set; } = 1;
    public string Description { get; set; } = "Description";
    public string BasicRules { get; set; } = "Rules for the game";

    public List<Game> Games { get; set; } = new List<Game>(); // Navigation property to Game
}   