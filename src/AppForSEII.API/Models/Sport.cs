namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class Sport
{

    public Sport() {

    }

    public Sport(string name, int minimumNumberofPlayers = 2, int numberofReferees = 1, string description, string basicRules) {
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

    public ICollection<Referee> Referees { get; set; } = new List<Referee>();

    public ICollection<Game> Games { get; set; } = new List<Game>();
}   