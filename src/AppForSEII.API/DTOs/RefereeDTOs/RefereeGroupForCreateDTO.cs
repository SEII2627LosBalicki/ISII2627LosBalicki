using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.RefereeDTOs;

public class RefereeGroupForCreateDTO
{
    public RefereeGroupForCreateDTO() { }

    public RefereeGroupForCreateDTO(int gameId, string name, string? description,
        string? rules, IList<RefereeAssignedToForCreateDTO> referees)
    {
        GameId = gameId;
        Name = name;
        Description = description;
        Rules = rules;
        Referees = referees;
    }

    [Required]
    public int GameId { get; set; }

    [Required]
    [StringLength(10, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? Rules { get; set; }

    public IList<RefereeAssignedToForCreateDTO> Referees { get; set; }
        = new List<RefereeAssignedToForCreateDTO>();

    public override bool Equals(object? obj)
    {
        return obj is RefereeGroupForCreateDTO dto &&
               GameId == dto.GameId &&
               Name == dto.Name &&
               Description == dto.Description &&
               Rules == dto.Rules &&
               Referees.SequenceEqual(dto.Referees);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GameId, Name, Description, Rules);
    }
}