using System;

public class Game
{
	public int Id { get; set; }
	public int hostTeamId { get; set; }
	public int guestTeamId { get; set; }
    public Team hostTeam { get; set; }
	public Team guestTeam { get; set; }
    public int weekNumber { get; set; }
	public int hostGoals { get; set; } = 0;
	public int guestGoals { get; set; } = 0;

    public ICollection<GameGoal> GameGoals { get; set; }

}
