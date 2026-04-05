namespace Football_League.DAL.Entities
{
    public class MatchGoalscorer
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public int GoalsCount { get; set; }

        // Navigation properties
        public Match Match { get; set; } = null!;
        public Player Player { get; set; } = null!;
    }
}
