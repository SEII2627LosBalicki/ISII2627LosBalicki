using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamInvitationDTO
{

    public TeamInvitationDTO()
    {
        
    }

    public TeamInvitationDTO(string? name, string? surname, int age, string invitationMessage)
    {
        Name = name;
        Surname = surname;
        Age = age;
        InvitationMessage = invitationMessage;
    }

    public string? Name { get; set; }
    public string? Surname { get; set; }
    public int Age { get; set; }
    [StringLength(250, MinimumLength = 3)]
    public string InvitationMessage { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is TeamInvitationDTO dTO &&
               Name == dTO.Name &&
               Surname == dTO.Surname &&
               Age == dTO.Age &&
               InvitationMessage == dTO.InvitationMessage;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Surname, Age, InvitationMessage);
    }

}