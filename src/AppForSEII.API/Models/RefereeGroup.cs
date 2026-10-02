public class RefereeGroup
{

    public RefereeGroup() {

    }

    public RefereeGroup(string name, int gameId, string? description = null, string? rules = null) {
        Name = name;
        Description = description;
        Rules = rules;
        GameId = gameId;
    }


    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 20 characters.")]
    public string Name { get; set; } = "Name between 3 and 20 chars.";

    public string? Description { get; set; }
    public string? Rules {get; set; }
    
    [Required]
    public int GameId { get; set; }
    [ForeignKey(nameof(GameId))]
    public Game Game { get; set; } = null!; // Navigation property to Game

    public ICollection<RefereeAssignedTo> RefereeAssignedTos { get; set; } = new List<RefereeAssignedTo>();
}