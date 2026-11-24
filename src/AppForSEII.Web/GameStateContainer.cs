using AppForSEII.Web.API;

namespace AppForSEII.Web;

public class GameStateContainer
{
    // We create an instance of Game when an instance of GameStateContainer is created
    public GameForCreateDTO Game { get; private set; } = new GameForCreateDTO()
    {
        GameInvitations = new List<TeamForGameDetailDTO>()
    };

    // We compute the number of teams invited to the game
    public int NumberOfTeams => Game.GameInvitations?.Count ?? 0;

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void AddTeamToGame(TeamForGameDTO team)
    {
        if (Game.GameInvitations == null)
        {
            Game.GameInvitations = new List<TeamForGameDetailDTO>();
        }

        // Before adding a team we check whether it has already been added
        if (!Game.GameInvitations.Any(ti => ti.Id == team.Id))
        {
            Game.GameInvitations.Add(new TeamForGameDetailDTO()
            {
                Id = team.Id,
                Name = team.Name,
                CaptainName = team.CaptainName,
                CaptainSurname = team.CaptainSurname,
                NumberOfMembers = team.NumberOfMembers,
                MinAge = team.MinAge,
                MaxAge = team.MaxAge,
                Sport = team.Sport,
                Message = "You are invited to play this game!"
            });
        }
    }

    // To delete a team from the list of invited teams
    public void RemoveTeamFromGame(TeamForGameDetailDTO team)
    {
        Game.GameInvitations?.Remove(team);
    }

    // We eliminate all the invited teams from the list
    public void ClearGameCart()
    {
        Game.GameInvitations?.Clear();
    }

    // We have already finished the process of creating the game, thus, we create a new Game
    public void GameProcessed()
    {
        Game = new GameForCreateDTO()
        {
            GameInvitations = new List<TeamForGameDetailDTO>()
        };
    }
}