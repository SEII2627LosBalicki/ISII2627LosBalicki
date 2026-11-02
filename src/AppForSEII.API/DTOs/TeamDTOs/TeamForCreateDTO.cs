using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamForCreateDTO
{

    public TeamForCreateDTO()
    {
        
    }

    public TeamForCreateDTO(string name, int maxMembers, string sportName, int maxAge, int minAge, string? description, string captainId, IList<TeamInvitationDTO> invitedPlayers)
    {
        Name = name;
        MaxMembers = maxMembers;
        SportName = sportName;
        MaxAge = maxAge;
        MinAge = minAge;
        Description = description;
        CaptainId = captainId;
        InvitedPlayers = invitedPlayers;
    }
    [StringLength(9, MinimumLength = 4)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [Range(2, 100)]
    public int MaxMembers { get; set; }
    [Required]
    public string SportName { get; set; } = string.Empty;
    [Range(3, 100)]
    public int MaxAge { get; set; } = 3;
    [Range(3, 100)]
    public int MinAge { get; set; } = 3;
    public string? Description { get; set; } = string.Empty;
    [Required]
    public string CaptainId { get; set; } = string.Empty;
    [Required]
    public IList<TeamInvitationDTO> InvitedPlayers { get; set; } = new List<TeamInvitationDTO>();


        public override bool Equals(object? obj)
    {
        return obj is TeamForCreateDTO dTO &&
               Name == dTO.Name &&
               MaxMembers == dTO.MaxMembers &&
               SportName == dTO.SportName &&
               MaxAge == dTO.MaxAge &&
               MinAge == dTO.MinAge &&
               Description == dTO.Description &&
               CaptainId == dTO.CaptainId &&
               InvitedPlayers.SequenceEqual(dTO.InvitedPlayers);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, MaxMembers, SportName, MaxAge, MinAge, Description, CaptainId);
    }
}

