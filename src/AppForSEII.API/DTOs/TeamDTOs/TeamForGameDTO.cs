namespace AppForSEII.API.DTOs.TeamDTOs;

public class TeamForGameDTO
{

    public TeamForGameDTO()
    {
        
    }

    public TeamForGameDTO(int id, string name, string? captainName, string? captainSurname, int numberOfMembers, int minAge, int maxAge, string sport)
    {
        Id = id;
        Name = name;
        CaptainName = captainName;
        CaptainSurname = captainSurname;
        NumberOfMembers = numberOfMembers;
        MinAge = minAge;
        MaxAge = maxAge;
        Sport = sport;
    }

    public int Id { get; set; }

        [StringLength(9, MinimumLength = 4)]
        public string Name { get; set; } = string.Empty;
        public string? CaptainName { get; set; }
        public string? CaptainSurname { get; set; }
        public int NumberOfMembers { get; set; }

        [Range(3, 100)]
        public int MinAge { get; set; }

        [Range(3, 100)]
        public int MaxAge { get; set; }
        public string Sport { get; set; } = string.Empty;
        

    public override bool Equals(object? obj)
    {
        return obj is TeamForGameDTO dTO &&
               Id == dTO.Id &&
               Name == dTO.Name &&
               CaptainName == dTO.CaptainName &&
               CaptainSurname == dTO.CaptainSurname &&
               NumberOfMembers == dTO.NumberOfMembers &&
               MinAge == dTO.MinAge &&
               MaxAge == dTO.MaxAge &&
               Sport == dTO.Sport;               
    }

    public override int GetHashCode()
    {
        HashCode hash = new HashCode();
        hash.Add(Id);
        hash.Add(Name);
        hash.Add(CaptainName);
        hash.Add(CaptainSurname);
        hash.Add(NumberOfMembers);
        hash.Add(MinAge);
        hash.Add(MaxAge);
        hash.Add(Sport);
        return hash.ToHashCode();
    }
}