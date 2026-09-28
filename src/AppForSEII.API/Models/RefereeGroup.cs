public class RefereeGroup
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 20 characters.")]
    public string Name { get; set; } = "Name between 3 and 20 chars.";

    public string? Description { get; set; }
    public string? Rules {get; set; }
    
    [Required]
    public int GameId { get; set; }
}