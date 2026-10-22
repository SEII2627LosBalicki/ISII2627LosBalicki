using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamDetailDTO
{

    public TeamDetailDTO()
    {
        
    }

    public TeamDetailDTO(int id, string name, string? captainName, string? captainSurname,string sportName, int minAge, int maxAge, int captainAge, IList<TeamInvitationDTO> invitedPlayers)
    {
        Id = id;
        Name = name;
        CaptainName = captainName;
        CaptainSurname = captainSurname;
        CaptainAge = captainAge;
        MinAge = minAge;
        MaxAge = maxAge;
        SportName = sportName;
        InvitedPlayers = invitedPlayers;
    }

    public int Id { get; set; }

    [StringLength(9, MinimumLength = 4)]
    public string Name { get; set; } = string.Empty;
    public string? CaptainName { get; set; }
    public string? CaptainSurname { get; set; }
    public int CaptainAge { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public string SportName { get; set; } = string.Empty;
    public IList<TeamInvitationDTO> InvitedPlayers { get; set; } = new List<TeamInvitationDTO>();

        public override bool Equals(object? obj)
    {
        return obj is TeamDetailDTO dTO &&
               Id == dTO.Id &&
               Name == dTO.Name &&
               CaptainName == dTO.CaptainName &&
               CaptainSurname == dTO.CaptainSurname &&
               CaptainAge == dTO.CaptainAge &&
               SportName == dTO.SportName &&
               MinAge == dTO.MinAge &&
               MaxAge == dTO.MaxAge &&
               InvitedPlayers.SequenceEqual(dTO.InvitedPlayers);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, CaptainName, CaptainSurname, CaptainAge, SportName, MinAge, MaxAge);
    }

}