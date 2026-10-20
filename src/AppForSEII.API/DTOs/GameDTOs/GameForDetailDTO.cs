using AppForSEII.API.DTOs.TeamDTOs;

namespace AppForSEII.API.DTOs.GameDTOs;

public class GameForDetailDTO
{
    public GameForDetailDTO()
    {
        
    }

    public GameForDetailDTO(int id, string name, string sport, DateTime date, string place, string? description, int numberOfTeams, IList<TeamForGameDetailDTO> teams)
    {
        Id = id;
        Name = name;
        Sport = sport;
        Date = date;
        Place = place;
        Description = description;
        NumberOfTeams = numberOfTeams;
        Teams = teams;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Sport { get; set; }
    public DateTime Date { get; set; }

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Place must have more than 5 characters.")] 
    public string Place { get; set; }
    public string? Description { get; set; }
    public int NumberOfTeams { get; set; }

    public IList<TeamForGameDetailDTO> Teams { get; set; } = new List<TeamForGameDetailDTO>();

    public override bool Equals(object? obj)
    {
        return obj is GameForDetailDTO dTO &&
               Id == dTO.Id &&
               Name == dTO.Name &&
               Sport == dTO.Sport &&
               Date == dTO.Date &&
               Place == dTO.Place &&
               Description == dTO.Description &&
               NumberOfTeams == dTO.NumberOfTeams &&
               Teams.SequenceEqual(dTO.Teams);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Sport, Date, Place, Description, NumberOfTeams, Teams);
    }
}