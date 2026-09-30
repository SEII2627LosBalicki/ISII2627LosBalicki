public class Referee : ApplicationUser {

    [Required]
    public int Rating { get; set; }

    [Required]
    public int YearsRefereeing { get; set; }
}