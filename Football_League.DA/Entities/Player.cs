namespace Football_League.DAL.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public int JerseyNumber { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int TeamId { get; set; }
        public int GoalsScored { get; set; }

        // Navigation properties
        public Team Team { get; set; } = null!;
        public ICollection<MatchGoalscorer> MatchGoalscorers { get; set; } = new List<MatchGoalscorer>();
    }
}
