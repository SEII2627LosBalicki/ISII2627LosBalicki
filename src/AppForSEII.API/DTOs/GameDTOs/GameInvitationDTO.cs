namespace AppForSEII.API.DTOs.GameDTOs;

public class GameInvitationDTO
{
    public GameInvitationDTO()
    {
        
    }

    public GameInvitationDTO(int teamId, string message)
    {
        TeamId = teamId;
        Message = message;
    }


    public int TeamId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 10, ErrorMessage = "Message must have more than 10 characters.")]
    public string Message { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is GameInvitationDTO dTO &&
               TeamId == dTO.TeamId &&
               Message == dTO.Message;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TeamId, Message);
    }
}