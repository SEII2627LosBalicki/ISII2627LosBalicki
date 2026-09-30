namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]

public class Game
{
    public Game(int id, string name, DateTime date, string place, string? description, int sportId, string responsibleforId)
    {
        Id = id;
        Name = name;
        Date = date;
        Place = place;
        Description = description;

        SportId = sportId;
        ResponsibleForId = responsibleforId;
    }

    public int Id { get; set; } // Primary key
    public string Name { get; set; }= "";
      
    public DateTime Date { get; set; }
    
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Place must have more than 5 characters.")]    
    public string Place { get; set; }= "More than 5 characters"; // Place must have more than 5 characters
    public string? Description { get; set;}
    
    public int SportId { get; set; } // Foreign key to Sport
    public Sport Sport { get; set; } = null!; // Navigation property to Sport

    public string ResponsibleForId { get; set; } // Foreign key to ApplicationUser
    public ApplicationUser ResponsibleFor { get; set; } = null!; // Navigation property to ApplicationUser

    public List<GameInvitation> GameInvitations { get; set; } = new List<GameInvitation>();// Navigation property to GameInvitation

}