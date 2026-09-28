namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
[PrimaryKey(nameof(Id),nameof(Name))]

public class Game
{
    public int Id { get; set; } // Primary key
    public string Name { get; set; }= "";      
    public DateTime Date { get; set; }
    
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Place must have more than 5 characters.")]    
    public string Place { get; set; }= "More than 5 characters"; // Place must have more than 5 characters
    public string? Description { get; set;}

}