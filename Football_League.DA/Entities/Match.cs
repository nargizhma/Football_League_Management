namespace Football_League.DAL.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int WeekNumber { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }

        // Navigation properties
        public Team HomeTeam { get; set; } = null!;
        public Team AwayTeam { get; set; } = null!;
        public ICollection<MatchGoalscorer> MatchGoalscorers { get; set; } = new List<MatchGoalscorer>();
    }
}
