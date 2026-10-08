using System.Data;
using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
    }
    public ApplicationUser(string id, string name, string surname, string userName)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
        Email = userName;
    }

    [StringLength(50)]
    public string? Name {get;set;}

    [StringLength(50)]
    public string? Surname {get;set;}
    public DateOnly BirthDate { get; set; }

    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - BirthDate.Year;
            if (BirthDate > today.AddYears(-age)) age--;
            return age;
        }
    }
    
    public Gender Gender { get; set; }

    public IList<Team> CaptainOf { get; set; } = new List<Team>();
    
    public IList<TeamInvitation> TeamInvitations { get; set; } = new List<TeamInvitation>();
    

    // Navigation property: Games where this user acts as captain/responsible (1:N)
    public ICollection<Game> ResponsibleOf { get; set; } = new List<Game>();
    
}
