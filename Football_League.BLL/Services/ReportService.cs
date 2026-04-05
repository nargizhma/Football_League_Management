using System.Text;
using Football_League.DAL.Repositories;

namespace Football_League.BLL.Services
{
    public class ReportService : IReportService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IPlayerRepository _playerRepository;

        public ReportService(ITeamRepository teamRepository, IPlayerRepository playerRepository)
        {
            _teamRepository = teamRepository;
            _playerRepository = playerRepository;
        }

        public async Task<string> GetTeamStandingsReportAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            var standings = teams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ThenBy(t => t.Name)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("\n╔════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                      LEAGUE STANDINGS                                ║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine("║ Team Name              │ P │ W │ D │ L │ GF │ GA │ GD  │ Pts │");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");

            foreach (var team in standings)
            {
                sb.AppendLine($"║ {team.Name,-22} │ {team.Played,1} │ {team.Wins,1} │ {team.Draws,1} │ {team.Losses,1} │ {team.GoalsFor,2} │ {team.GoalsAgainst,2} │ {team.GoalDifference,3} │ {team.Points,3} │");
            }

            sb.AppendLine("╚════════════════════════════════════════════════════════════════════╝\n");
            return sb.ToString();
        }

        public async Task<string> GetTeamStatisticsReportAsync(int teamId)
        {
            var team = await _teamRepository.GetByIdWithDetailsAsync(teamId);
            if (team == null)
                return $"Team with ID {teamId} not found";

            var sb = new StringBuilder();
            sb.AppendLine($"\n╔════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine($"║                   {team.Name,-48}║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║ Code: {team.Code,-4} │ Stadium: {team.Stadium?.Name,-44}║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║ Played │ Wins │ Draws │ Losses │ Goals For │ Goals Against │ GD │ Points │");
            sb.AppendLine($"║ {team.Played,-6} │ {team.Wins,-4} │ {team.Draws,-5} │ {team.Losses,-6} │ {team.GoalsFor,-8} │ {team.GoalsAgainst,-13} │ {team.GoalDifference,-2} │ {team.Points,-6} │");
            sb.AppendLine("╚════════════════════════════════════════════════════════════════════╝\n");

            return sb.ToString();
        }

        public async Task<string> GetTeamPlayersReportAsync(int teamId)
        {
            var team = await _teamRepository.GetByIdWithDetailsAsync(teamId);
            if (team == null)
                return $"Team with ID {teamId} not found";

            var sb = new StringBuilder();
            sb.AppendLine($"\n╔════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine($"║            {team.Name,-48} - PLAYERS");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine($"║ Jersey │ Full Name                        │ Goals Scored │");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");

            var players = team.Players.OrderBy(p => p.JerseyNumber);
            if (!players.Any())
            {
                sb.AppendLine("║ No players found for this team                                     ║");
            }
            else
            {
                foreach (var player in players)
                {
                    sb.AppendLine($"║ {player.JerseyNumber,-6} │ {player.FullName,-34} │ {player.GoalsScored,-12} │");
                }
            }

            sb.AppendLine("╚════════════════════════════════════════════════════════════════════╝\n");
            return sb.ToString();
        }

        public async Task<string> GetTopScorersReportAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            var topScorers = players
                .Where(p => p.GoalsScored > 0)
                .OrderByDescending(p => p.GoalsScored)
                .ThenBy(p => p.FullName)
                .Take(20)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("\n╔════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                         TOP SCORERS                                ║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine("║ Pos │ Team              │ Jersey │ Full Name              │ Goals │");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");

            if (!topScorers.Any())
            {
                sb.AppendLine("║ No scorers yet                                                     ║");
            }
            else
            {
                int position = 1;
                foreach (var player in topScorers)
                {
                    var team = player.Team ?? new DAL.Entities.Team { Name = "Unknown" };
                    sb.AppendLine($"║ {position,-3} │ {team.Name,-17} │ {player.JerseyNumber,-6} │ {player.FullName,-22} │ {player.GoalsScored,-5} │");
                    position++;
                }
            }

            sb.AppendLine("╚════════════════════════════════════════════════════════════════════╝\n");
            return sb.ToString();
        }

        public async Task<string> GetMostGoalsConcededReportAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            var sortedTeams = teams
                .OrderByDescending(t => t.GoalsAgainst)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("\n╔════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                   TEAMS WITH MOST GOALS CONCEDED                  ║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine("║ Pos │ Team Name              │ Goals Conceded │");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");

            if (!sortedTeams.Any())
            {
                sb.AppendLine("║ No data available                                                  ║");
            }
            else
            {
                int position = 1;
                foreach (var team in sortedTeams)
                {
                    sb.AppendLine($"║ {position,-3} │ {team.Name,-22} │ {team.GoalsAgainst,-14} │");
                    position++;
                }
            }

            sb.AppendLine("╚════════════════════════════════════════════════════════════════════╝\n");
            return sb.ToString();
        }

        public async Task<string> GetMostGoalsScoredReportAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            var sortedTeams = teams
                .OrderByDescending(t => t.GoalsFor)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("\n╔════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║                    TEAMS WITH MOST GOALS SCORED                   ║");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");
            sb.AppendLine("║ Pos │ Team Name              │ Goals Scored │");
            sb.AppendLine("╠════════════════════════════════════════════════════════════════════╣");

            if (!sortedTeams.Any())
            {
                sb.AppendLine("║ No data available                                                  ║");
            }
            else
            {
                int position = 1;
                foreach (var team in sortedTeams)
                {
                    sb.AppendLine($"║ {position,-3} │ {team.Name,-22} │ {team.GoalsFor,-12} │");
                    position++;
                }
            }

            sb.AppendLine("╚════════════════════════════════════════════════════════════════════╝\n");
            return sb.ToString();
        }
    }
}
