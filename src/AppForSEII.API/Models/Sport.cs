namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
public class Sport
{
    public int Id { get; set; }
    public string Name { get; set; } = "Game";
    public int MinimumNumberofPlayers { get; set; } = 2;
    public int NumberofReferees { get; set; } = 1;
    public string Description { get; set; } = "Description";
    public string BasicRules { get; set; } = "Rules for the game";
}