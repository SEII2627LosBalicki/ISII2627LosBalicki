namespace AppForSEII.API.Models;

[Index(nameof(Name), IsUnique = true)]
[PrimaryKey(nameof(Id),nameof(Name))]

public class Game
{
    public int Id { get; set; } // Primary key
    public string Name { get; set; }= "Game name";      
    public DateTime Date { get; set; }
    public string Place { get; set; }
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Place must have more than 5 characters.")]
    public string Message { get; set; }
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Message must have more than 10 characters.")]
    public string? Description { get; set;}

}