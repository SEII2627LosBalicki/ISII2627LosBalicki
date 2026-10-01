namespace AppForSEII.API.Models
{
    public class Sport
    {
        public int Id { get; set; }

        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}