namespace AppForSEII.API.DTOs.GameDTOs;

public class GameForCreateDTO
{
    public GameForCreateDTO()
    {
        
    }

    public GameForCreateDTO(string name, DateTime date, string place, string? description, string sportName, string responsibleUserName, IList<GameInvitationDTO> gameInvitations)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Date = date;
        Place = place ?? throw new ArgumentNullException(nameof(place));
        Description = description;
        SportName = sportName ?? throw new ArgumentNullException(nameof(sportName));
        ResponsibleUserName = responsibleUserName ?? throw new ArgumentNullException(nameof(responsibleUserName));
        GameInvitations = gameInvitations;
    }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set the game name")]
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set the place")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Place must have more than 5 characters.")]
    public string Place { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set the sport name")]
    public string SportName { get; set; } = string.Empty;

    [EmailAddress]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set the responsible user email")]
    public string ResponsibleUserName { get; set; } = string.Empty;
    public IList<GameInvitationDTO> GameInvitations { get; set; } = new List<GameInvitationDTO>();

    protected bool CompareDate(DateTime date1, DateTime date2)
    {
        return date1.Subtract(date2) < new TimeSpan(0, 1, 0);
    }

    public override bool Equals(object? obj)
    {
        return obj is GameForCreateDTO dTO &&
               Name == dTO.Name &&
               Date == dTO.Date &&
               Place == dTO.Place &&
               Description == dTO.Description &&
               SportName == dTO.SportName &&
               ResponsibleUserName == dTO.ResponsibleUserName &&
               GameInvitations.SequenceEqual(dTO.GameInvitations) &&
               CompareDate(Date, dTO.Date);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Date, Place, Description, SportName, ResponsibleUserName, GameInvitations);
    }
}