namespace AppForSEII.API.Models;

[PrimaryKey(nameof(TeamId), nameof(GameId))]

public class GameInvitation
{
    public GameInvitation(int teamId, int gameId, string message)
    {
        TeamId = teamId;
        GameId = gameId;
        Message = message;
    }

    public int TeamId { get; set; }
    public int GameId { get; set; }
    public bool AcceptedGame { get; set; } = false;

    [StringLength(100, MinimumLength = 11, ErrorMessage = "Message must have more than 10 characters.")]
    public string Message { get; set; } = "Message must have more than 10 characters."; // Message must have more than 10 characters

    // Navigation properties    
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Team TeamInvited { get; set; } = null!; // Navigation property to Team
    public Game GameInvited { get; set; } = null!; // Navigation property to Game
}