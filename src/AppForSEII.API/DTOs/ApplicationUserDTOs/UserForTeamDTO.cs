using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.DTOs.ApplicationUserDTO;

public class UserForTeamDTO 
{
    public UserForTeamDTO()
    {
    }
    public UserForTeamDTO(string id, string? name, string? surname, int age, IList<SportSkillDTO> sports)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Age = age;
        Sports = sports;
    }

    [StringLength(50)]
    public string? Id {get;set;}

    [StringLength(50)]
    public string? Name{ get;set;}

    [StringLength(50)]
    public string? Surname {get;set;}

    public int Age { get; set; }
    public IList<SportSkillDTO> Sports { get; set; } = new List<SportSkillDTO>();

    public override bool Equals(object? obj)
    {
        return obj is UserForTeamDTO dTO &&
               Id == dTO.Id &&
               Name == dTO.Name &&
               Surname == dTO.Surname &&
               Age == dTO.Age &&
               Sports.SequenceEqual(dTO.Sports);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Surname, Age);
    }

}