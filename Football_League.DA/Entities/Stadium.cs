namespace Football_League.DAL.Entities
{
    public class Stadium
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }

        // Navigation property
        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
