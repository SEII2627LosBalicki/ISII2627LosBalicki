public class Referee : ApplicationUser {

    //Empty Constructor
    public Referee() {

    }

    //Constructor with parameters
    public Referee(string id, string name, string surname, string userName, int rating, int yearsRefereeing) 
        : base(id, name, surname, userName) 
    {
        Rating = rating;
        YearsRefereeing = yearsRefereeing;
    }

    [Required]
    public int Rating { get; set; }

    [Required]
    public int YearsRefereeing { get; set; }

    public int? SportId { get; set; }

    public Sport? SportForReferee { get; set; }

    public ICollection<RefereeAssignedTo> RefereeAssignedTos { get; set; } = new List<RefereeAssignedTo>();
}