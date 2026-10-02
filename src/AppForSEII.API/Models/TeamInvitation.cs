using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII.API.Models;

[PrimaryKey(nameof(UserId), nameof(TeamId))]
public class TeamInvitation
{
    public TeamInvitation()
    {
    }

    public TeamInvitation(string userId, int teamId, string invitationMessage, bool invitationAccepted = false)
    {
        UserId = userId;
        TeamId = teamId;
        InvitationAccepted = invitationAccepted;
        InvitationMessage = invitationMessage;

    }

    public string UserId { get; set; } = "0";

    public ApplicationUser User { get; set; } = null!;

    public int TeamId { get; set; }
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Team Team { get; set; } = null!;

    public bool InvitationAccepted { get; set; } = false;
    [Required]
    [StringLength(250, MinimumLength = 3)]
    public string InvitationMessage { get; set; } = "Invitation Message for a team member";
}