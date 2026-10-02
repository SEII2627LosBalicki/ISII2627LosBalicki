namespace AppForSEII.API.Models;
using Microsoft.EntityFrameworkCore;

public class RefereeAssignedTo
{

    public RefereeAssignedTo() {

    }

    public RefereeAssignedTo(int refereeGroupId, string refereeId, string role, string roleDescription, bool acceptedAssignment = false) {
        RefereeGroupId = refereeGroupId;
        RefereeId = refereeId;
        Role = role;
        RoleDescription = roleDescription;
        AcceptedAssignment = acceptedAssignment;
    }

    [Key]
    public int Id { get; set; }
    
    [Required]
    public int RefereeGroupId { get; set; }

    [Required]
    public string RefereeId { get; set; } = "0";

    public bool AcceptedAssignment { get; set; } = false;

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Role must be between 3 and 20 characters.")]
    public string Role { get; set; } = "Name of the role has to be between 3 and 20 chars.";

    [StringLength(255, MinimumLength = 3, ErrorMessage = "Between 3 and 255 characters.")]
    public string RoleDescription { get; set; } = "Between 3 and 255 chars.";

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public RefereeGroup? RefereeGroup { get; set; }
    public Referee? Referee { get; set; }
}