using AppForSEII.API.DTOs.RefereeAssignedToDTOs;

namespace AppForSEII.API.DTOs.RefereeGroupDTOs;

public class RefereeGroupDTO
{
    public RefereeGroupDTO() { }

    public RefereeGroupDTO(string gameName, DateTime gameDate, string gamePlace, string sport,
        string name, string? description, string? rules,
        IList<RefereeAssignedToDTO> refereeAssignedToDTOs)
    {
        GameName = gameName;
        GameDate = gameDate;
        GamePlace = gamePlace;
        Sport = sport;
        Name = name;
        Description = description;
        Rules = rules;
        RefereeAssignedToDTOs = refereeAssignedToDTOs;
    }

    public string GameName { get; set; } = string.Empty;

    public DateTime GameDate { get; set; }

    [StringLength(100, MinimumLength = 6)]
    public string GamePlace { get; set; } = string.Empty;

    public string Sport { get; set; } = string.Empty;

    [StringLength(10, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Rules { get; set; }

    public IList<RefereeAssignedToDTO> RefereeAssignedToDTOs { get; set; } = new List<RefereeAssignedToDTO>();

    public override bool Equals(object? obj)
    {
        return obj is RefereeGroupDTO dto &&
               GameName == dto.GameName &&
               GameDate == dto.GameDate &&
               GamePlace == dto.GamePlace &&
               Sport == dto.Sport &&
               Name == dto.Name &&
               Description == dto.Description &&
               Rules == dto.Rules &&
               RefereeAssignedToDTOs.SequenceEqual(dto.RefereeAssignedToDTOs);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GameName, GameDate, GamePlace, Sport, Name, Description, Rules);
    }
}