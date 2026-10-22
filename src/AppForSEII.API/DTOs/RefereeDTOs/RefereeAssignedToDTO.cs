namespace AppForSEII.API.DTOs.RefereeAssignedToDTOs;

public class RefereeAssignedToDTO
{
    public RefereeAssignedToDTO() { }

    public RefereeAssignedToDTO(string? name, string? surname, int yearsRefereeing,
        string role, string roleDescription)
    {
        Name = name;
        Surname = surname;
        YearsRefereeing = yearsRefereeing;
        Role = role;
        RoleDescription = roleDescription;
    }

    [StringLength(50)]
    public string? Name { get; set; }

    [StringLength(50)]
    public string? Surname { get; set; }

    [Range(1, int.MaxValue)]
    public int YearsRefereeing { get; set; }

    [StringLength(20, MinimumLength = 3)]
    public string Role { get; set; } = string.Empty;

    [StringLength(255, MinimumLength = 3)]
    public string RoleDescription { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is RefereeAssignedToDTO dto &&
               Name == dto.Name &&
               Surname == dto.Surname &&
               YearsRefereeing == dto.YearsRefereeing &&
               Role == dto.Role &&
               RoleDescription == dto.RoleDescription;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Surname, YearsRefereeing, Role, RoleDescription);
    }
}