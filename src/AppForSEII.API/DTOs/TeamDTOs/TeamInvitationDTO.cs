using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamInvitationDTO
{

    public TeamInvitationDTO()
    {
        
    }

    public TeamInvitationDTO(string? name, string? surname, string id, int age, string invitationMessage)
    {
        Name = name;
        Surname = surname;
        Age = age;
        InvitationMessage = invitationMessage;
        Id = id;
    }

    public string? Name { get; set; }
    public string? Surname { get; set; }
    [Required]
    public string Id { get; set; } = string.Empty;
    public int Age { get; set; }
    [StringLength(250, MinimumLength = 3)]
    public string InvitationMessage { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is TeamInvitationDTO dTO &&
               Name == dTO.Name &&
               Surname == dTO.Surname &&
               Age == dTO.Age &&
               InvitationMessage == dTO.InvitationMessage &&
               Id == dTO.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Surname, Age, InvitationMessage, Id);
    }

}