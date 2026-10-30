using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.RefereeDTOs;

public class RefereeAssignedToForCreateDTO
{
    public RefereeAssignedToForCreateDTO() { }

    public RefereeAssignedToForCreateDTO(string refereeId, string role, string roleDescription)
    {
        RefereeId = refereeId;
        Role = role;
        RoleDescription = roleDescription;
    }

    [Required]
    public string RefereeId { get; set; } = string.Empty;

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 3)]
    public string RoleDescription { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is RefereeAssignedToForCreateDTO dto &&
               RefereeId == dto.RefereeId &&
               Role == dto.Role &&
               RoleDescription == dto.RoleDescription;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RefereeId, Role, RoleDescription);
    }
}