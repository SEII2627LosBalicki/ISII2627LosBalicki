
namespace AppForSEII.API.DTOs.RefereeDTOs;

public class RefereeForRefereeGroupDTO
{
    public RefereeForRefereeGroupDTO() { }

    public RefereeForRefereeGroupDTO(string id, string? name, string? surname,
        int yearsRefereeing, int rating, string sport)
    {
        Id = id;
        Name = name;
        Surname = surname;
        YearsRefereeing = yearsRefereeing;
        Rating = rating;
        Sport = sport;
    }

    public string Id { get; set; } = string.Empty;
    [StringLength(50)]
    public string? Name { get; set; }
    [StringLength(50)]
    public string? Surname { get; set; }
    [Range(1, int.MaxValue)]
    public int YearsRefereeing { get; set; }
    [Range(1, 5)]
    public int Rating { get; set; }
    [StringLength(50)]
    public string Sport { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is RefereeForRefereeGroupDTO dto &&
               Id == dto.Id &&
               Name == dto.Name &&
               Surname == dto.Surname &&
               YearsRefereeing == dto.YearsRefereeing &&
               Rating == dto.Rating &&
               Sport == dto.Sport;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Surname, YearsRefereeing, Rating, Sport);
    }
}