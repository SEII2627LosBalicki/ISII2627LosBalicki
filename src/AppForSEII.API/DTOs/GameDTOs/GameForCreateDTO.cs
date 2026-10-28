namespace AppForSEII.API.DTOs.GameDTOs;

public class GameForCreateDTO
{
    public GameForCreateDTO()
    {
        
    }
    public GameForCreateDTO(string name, DateTime date, string place, string? description, string sportName, string responsibleId, IList<GameInvitationDTO> gameInvitations)
    {
        Name = name;
        Date = date;
        Place = place;
        Description = description;
        SportName = sportName;
        ResponsibleId = responsibleId;
        GameInvitations = gameInvitations;
    }

    [Required]
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Place must have more than 5 characters.")]
    public string Place { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    [Required]
    public string SportName { get; set; } = string.Empty;

    [Required]
    public string ResponsibleId { get; set; } = string.Empty;
    public IList<GameInvitationDTO> GameInvitations { get; set; } = new List<GameInvitationDTO>();

    public override bool Equals(object? obj)
    {
        return obj is GameForCreateDTO dTO &&
               Name == dTO.Name &&
               Date == dTO.Date &&
               Place == dTO.Place &&
               Description == dTO.Description &&
               SportName == dTO.SportName &&
               ResponsibleId == dTO.ResponsibleId &&
               GameInvitations.SequenceEqual(dTO.GameInvitations);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Date, Place, Description, SportName, ResponsibleId, GameInvitations);
    }
}