namespace Football_League.DAL.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Code { get; set; }
        public int StadiumId { get; set; }

        // Statistics
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int Points { get; set; }

        // Computed property
        public int GoalDifference => GoalsFor - GoalsAgainst;

        // Navigation properties
        public Stadium Stadium { get; set; } = null!;
        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}
