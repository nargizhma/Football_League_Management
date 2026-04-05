using Football_League.DAL.Entities;
using Football_League.DAL.Repositories;

namespace Football_League.BLL.Services
{
    public class MatchService : IMatchService
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IMatchGoalscorerRepository _matchGoalscorerRepository;

        public MatchService(
            IMatchRepository matchRepository,
            ITeamRepository teamRepository,
            IPlayerRepository playerRepository,
            IMatchGoalscorerRepository matchGoalscorerRepository)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _playerRepository = playerRepository;
            _matchGoalscorerRepository = matchGoalscorerRepository;
        }

        public async Task<Match?> GetMatchByIdAsync(int id)
        {
            return await _matchRepository.GetByIdAsync(id);
        }

        public async Task<Match?> GetMatchByIdWithDetailsAsync(int id)
        {
            return await _matchRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Match>> GetAllMatchesAsync()
        {
            return await _matchRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync()
        {
            return await _matchRepository.GetAllWithDetailsAsync();
        }

        public async Task<int> CreateMatchAsync(
            int homeTeamId,
            int awayTeamId,
            int weekNumber,
            int homeGoals,
            int awayGoals,
            IEnumerable<(int PlayerId, int Goals)>? goalscorers = null)
        {
            if (homeTeamId == awayTeamId)
                throw new InvalidOperationException("Home team and away team cannot be the same");
            if (weekNumber < 1)
                throw new ArgumentException("Week number must be 1 or greater");
            if (homeGoals < 0 || awayGoals < 0)
                throw new ArgumentException("Goals cannot be negative");

            var homeTeam = await _teamRepository.GetByIdAsync(homeTeamId);
            var awayTeam = await _teamRepository.GetByIdAsync(awayTeamId);

            if (homeTeam == null)
                throw new KeyNotFoundException($"Home team with ID {homeTeamId} not found");
            if (awayTeam == null)
                throw new KeyNotFoundException($"Away team with ID {awayTeamId} not found");

            try
            {
                var match = new Match
                {
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    WeekNumber = weekNumber,
                    HomeGoals = homeGoals,
                    AwayGoals = awayGoals
                };

                await _matchRepository.AddAsync(match);
                await _matchRepository.SaveAsync();

                await UpdateTeamStatisticsAsync(homeTeam, awayTeam, homeGoals, awayGoals);

                if (goalscorers != null && goalscorers.Any())
                {
                    await AddGoalscorersAsync(match.Id, homeTeamId, awayTeamId, goalscorers);
                }

                return match.Id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create match: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task UpdateMatchAsync(
            int id,
            int homeGoals,
            int awayGoals,
            IEnumerable<(int PlayerId, int Goals)>? goalscorers = null)
        {
            if (homeGoals < 0 || awayGoals < 0)
                throw new ArgumentException("Goals cannot be negative");

            var match = await _matchRepository.GetByIdWithDetailsAsync(id);
            if (match == null)
                throw new KeyNotFoundException($"Match with ID {id} not found");

            var homeTeam = await _teamRepository.GetByIdAsync(match.HomeTeamId);
            var awayTeam = await _teamRepository.GetByIdAsync(match.AwayTeamId);

            // Recalculate standings from scratch
            await RecalculateTeamStatisticsAsync(homeTeam!, awayTeam!);

            // Update the match
            match.HomeGoals = homeGoals;
            match.AwayGoals = awayGoals;
            await _matchRepository.UpdateAsync(match);

            // Update statistics again with new goals
            await UpdateTeamStatisticsAsync(homeTeam!, awayTeam!, homeGoals, awayGoals);

            // Update goalscorers if provided
            if (goalscorers != null)
            {
                var existingGoalscorers = await _matchGoalscorerRepository.GetByMatchIdAsync(id);
                foreach (var existing in existingGoalscorers)
                {
                    await _matchGoalscorerRepository.DeleteAsync(existing.Id);
                }
                await _matchGoalscorerRepository.SaveAsync();

                if (goalscorers.Any())
                {
                    await AddGoalscorersAsync(id, match.HomeTeamId, match.AwayTeamId, goalscorers);
                }
            }

            await _matchRepository.SaveAsync();
        }

        public async Task DeleteMatchAsync(int id)
        {
            var match = await _matchRepository.GetByIdWithDetailsAsync(id);
            if (match == null)
                throw new KeyNotFoundException($"Match with ID {id} not found");

            var homeTeam = await _teamRepository.GetByIdAsync(match.HomeTeamId);
            var awayTeam = await _teamRepository.GetByIdAsync(match.AwayTeamId);

            // Remove goalscorers first
            var goalscorers = await _matchGoalscorerRepository.GetByMatchIdAsync(id);
            foreach (var goalscorer in goalscorers)
            {
                await _matchGoalscorerRepository.DeleteAsync(goalscorer.Id);
            }
            await _matchGoalscorerRepository.SaveAsync();

            // Delete the match
            await _matchRepository.DeleteAsync(id);
            await _matchRepository.SaveAsync();

            // Recalculate standings
            await RecalculateTeamStatisticsAsync(homeTeam!, awayTeam!);
        }

        private async Task UpdateTeamStatisticsAsync(Team homeTeam, Team awayTeam, int homeGoals, int awayGoals)
        {
            homeTeam.Played++;
            awayTeam.Played++;

            homeTeam.GoalsFor += homeGoals;
            homeTeam.GoalsAgainst += awayGoals;

            awayTeam.GoalsFor += awayGoals;
            awayTeam.GoalsAgainst += homeGoals;

            if (homeGoals > awayGoals)
            {
                homeTeam.Wins++;
                homeTeam.Points += 3;
                awayTeam.Losses++;
            }
            else if (awayGoals > homeGoals)
            {
                awayTeam.Wins++;
                awayTeam.Points += 3;
                homeTeam.Losses++;
            }
            else
            {
                homeTeam.Draws++;
                homeTeam.Points += 1;
                awayTeam.Draws++;
                awayTeam.Points += 1;
            }

            await _teamRepository.UpdateAsync(homeTeam);
            await _teamRepository.UpdateAsync(awayTeam);
        }

        private async Task RecalculateTeamStatisticsAsync(Team homeTeam, Team awayTeam)
        {
            homeTeam.Played = 0;
            homeTeam.Wins = 0;
            homeTeam.Draws = 0;
            homeTeam.Losses = 0;
            homeTeam.GoalsFor = 0;
            homeTeam.GoalsAgainst = 0;
            homeTeam.Points = 0;

            awayTeam.Played = 0;
            awayTeam.Wins = 0;
            awayTeam.Draws = 0;
            awayTeam.Losses = 0;
            awayTeam.GoalsFor = 0;
            awayTeam.GoalsAgainst = 0;
            awayTeam.Points = 0;

            var allMatches = await _matchRepository.GetAllAsync();
            foreach (var match in allMatches)
            {
                if (match.HomeTeamId == homeTeam.Id || match.AwayTeamId == homeTeam.Id ||
                    match.HomeTeamId == awayTeam.Id || match.AwayTeamId == awayTeam.Id)
                {
                    if (match.HomeTeamId == homeTeam.Id)
                    {
                        homeTeam.Played++;
                        homeTeam.GoalsFor += match.HomeGoals;
                        homeTeam.GoalsAgainst += match.AwayGoals;

                        if (match.HomeGoals > match.AwayGoals)
                        {
                            homeTeam.Wins++;
                            homeTeam.Points += 3;
                        }
                        else if (match.HomeGoals < match.AwayGoals)
                        {
                            homeTeam.Losses++;
                        }
                        else
                        {
                            homeTeam.Draws++;
                            homeTeam.Points += 1;
                        }
                    }

                    if (match.AwayTeamId == homeTeam.Id)
                    {
                        homeTeam.Played++;
                        homeTeam.GoalsFor += match.AwayGoals;
                        homeTeam.GoalsAgainst += match.HomeGoals;

                        if (match.AwayGoals > match.HomeGoals)
                        {
                            homeTeam.Wins++;
                            homeTeam.Points += 3;
                        }
                        else if (match.AwayGoals < match.HomeGoals)
                        {
                            homeTeam.Losses++;
                        }
                        else
                        {
                            homeTeam.Draws++;
                            homeTeam.Points += 1;
                        }
                    }

                    if (match.HomeTeamId == awayTeam.Id)
                    {
                        awayTeam.Played++;
                        awayTeam.GoalsFor += match.HomeGoals;
                        awayTeam.GoalsAgainst += match.AwayGoals;

                        if (match.HomeGoals > match.AwayGoals)
                        {
                            awayTeam.Wins++;
                            awayTeam.Points += 3;
                        }
                        else if (match.HomeGoals < match.AwayGoals)
                        {
                            awayTeam.Losses++;
                        }
                        else
                        {
                            awayTeam.Draws++;
                            awayTeam.Points += 1;
                        }
                    }

                    if (match.AwayTeamId == awayTeam.Id)
                    {
                        awayTeam.Played++;
                        awayTeam.GoalsFor += match.AwayGoals;
                        awayTeam.GoalsAgainst += match.HomeGoals;

                        if (match.AwayGoals > match.HomeGoals)
                        {
                            awayTeam.Wins++;
                            awayTeam.Points += 3;
                        }
                        else if (match.AwayGoals < match.HomeGoals)
                        {
                            awayTeam.Losses++;
                        }
                        else
                        {
                            awayTeam.Draws++;
                            awayTeam.Points += 1;
                        }
                    }
                }
            }

            await _teamRepository.UpdateAsync(homeTeam);
            await _teamRepository.UpdateAsync(awayTeam);
            await _teamRepository.SaveAsync();
        }

        private async Task AddGoalscorersAsync(
            int matchId,
            int homeTeamId,
            int awayTeamId,
            IEnumerable<(int PlayerId, int Goals)> goalscorers)
        {
            foreach (var (playerId, goals) in goalscorers)
            {
                if (goals < 1)
                    continue;

                var player = await _playerRepository.GetByIdAsync(playerId);
                if (player == null)
                    throw new KeyNotFoundException($"Player with ID {playerId} not found");

                if (player.TeamId != homeTeamId && player.TeamId != awayTeamId)
                    throw new InvalidOperationException("Player does not belong to either team in this match");

                var goalscorer = new MatchGoalscorer
                {
                    MatchId = matchId,
                    PlayerId = playerId,
                    GoalsCount = goals
                };

                player.GoalsScored += goals;
                await _playerRepository.UpdateAsync(player);
                await _matchGoalscorerRepository.AddAsync(goalscorer);
            }

            await _matchGoalscorerRepository.SaveAsync();
        }
    }
}
