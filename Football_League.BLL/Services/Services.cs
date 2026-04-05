using Football_League.DAL.Entities;
using Football_League.DAL.Repositories;

namespace Football_League.BLL.Services
{
    public class StadiumService : IStadiumService
    {
        private readonly IStadiumRepository _repository;

        public StadiumService(IStadiumRepository repository)
        {
            _repository = repository;
        }

        public async Task<Stadium?> GetStadiumByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Stadium>> GetAllStadiumsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<int> CreateStadiumAsync(string name, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new ArgumentException("Stadium name must be 1-100 characters");
            if (capacity <= 0)
                throw new ArgumentException("Stadium capacity must be greater than 0");

            try
            {
                var stadium = new Stadium { Name = name, Capacity = capacity };
                await _repository.AddAsync(stadium);
                await _repository.SaveAsync();
                return stadium.Id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create stadium: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task UpdateStadiumAsync(int id, string name, int capacity)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new ArgumentException("Stadium name must be 1-100 characters");
            if (capacity <= 0)
                throw new ArgumentException("Stadium capacity must be greater than 0");

            var stadium = await _repository.GetByIdAsync(id);
            if (stadium == null)
                throw new KeyNotFoundException($"Stadium with ID {id} not found");

            try
            {
                stadium.Name = name;
                stadium.Capacity = capacity;
                await _repository.UpdateAsync(stadium);
                await _repository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to update stadium: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task DeleteStadiumAsync(int id)
        {
            var stadium = await _repository.GetByIdAsync(id);
            if (stadium == null)
                throw new KeyNotFoundException($"Stadium with ID {id} not found");

            await _repository.DeleteAsync(id);
            await _repository.SaveAsync();
        }
    }

    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IStadiumRepository _stadiumRepository;

        public TeamService(ITeamRepository teamRepository, IStadiumRepository stadiumRepository)
        {
            _teamRepository = teamRepository;
            _stadiumRepository = stadiumRepository;
        }

        public async Task<Team?> GetTeamByIdAsync(int id)
        {
            return await _teamRepository.GetByIdAsync(id);
        }

        public async Task<Team?> GetTeamByIdWithDetailsAsync(int id)
        {
            return await _teamRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _teamRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Team>> GetAllTeamsWithDetailsAsync()
        {
            return await _teamRepository.GetAllWithDetailsAsync();
        }

        public async Task<int> CreateTeamAsync(string name, int code, int stadiumId)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new ArgumentException("Team name must be 1-100 characters");
            if (code < 1 || code > 99)
                throw new ArgumentException("Team code must be between 1 and 99");

            if (!await _teamRepository.IsTeamNameUniqueAsync(name))
                throw new InvalidOperationException("Team name must be unique");
            if (!await _teamRepository.IsTeamCodeUniqueAsync(code))
                throw new InvalidOperationException("Team code must be unique");

            var stadium = await _stadiumRepository.GetByIdAsync(stadiumId);
            if (stadium == null)
                throw new KeyNotFoundException($"Stadium with ID {stadiumId} not found");

            try
            {
                var team = new Team
                {
                    Name = name,
                    Code = code,
                    StadiumId = stadiumId,
                    Played = 0,
                    Wins = 0,
                    Draws = 0,
                    Losses = 0,
                    GoalsFor = 0,
                    GoalsAgainst = 0,
                    Points = 0
                };

                await _teamRepository.AddAsync(team);
                await _teamRepository.SaveAsync();
                return team.Id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create team: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task UpdateTeamAsync(int id, string name, int code, int stadiumId)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length > 100)
                throw new ArgumentException("Team name must be 1-100 characters");
            if (code < 1 || code > 99)
                throw new ArgumentException("Team code must be between 1 and 99");

            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {id} not found");

            if (!await _teamRepository.IsTeamNameUniqueAsync(name, id))
                throw new InvalidOperationException("Team name must be unique");
            if (!await _teamRepository.IsTeamCodeUniqueAsync(code, id))
                throw new InvalidOperationException("Team code must be unique");

            var stadium = await _stadiumRepository.GetByIdAsync(stadiumId);
            if (stadium == null)
                throw new KeyNotFoundException($"Stadium with ID {stadiumId} not found");

            team.Name = name;
            team.Code = code;
            team.StadiumId = stadiumId;

            await _teamRepository.UpdateAsync(team);
            await _teamRepository.SaveAsync();
        }

        public async Task DeleteTeamAsync(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {id} not found");

            await _teamRepository.DeleteAsync(id);
            await _teamRepository.SaveAsync();
        }

        public async Task<IEnumerable<Team>> GetStandingsAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            return teams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ThenBy(t => t.Name);
        }
    }

    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;

        public PlayerService(IPlayerRepository playerRepository, ITeamRepository teamRepository)
        {
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _playerRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Player>> GetPlayersByTeamIdAsync(int teamId)
        {
            return await _playerRepository.GetByTeamIdAsync(teamId);
        }

        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _playerRepository.GetAllAsync();
        }

        public async Task<int> CreatePlayerAsync(int jerseyNumber, string fullName, int teamId)
        {
            if (jerseyNumber < 1 || jerseyNumber > 99)
                throw new ArgumentException("Jersey number must be between 1 and 99");
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 100)
                throw new ArgumentException("Full name must be 1-100 characters");

            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            if (!await _playerRepository.IsJerseyNumberUniqueInTeamAsync(teamId, jerseyNumber))
                throw new InvalidOperationException("Jersey number already exists in this team");

            try
            {
                var player = new Player
                {
                    JerseyNumber = jerseyNumber,
                    FullName = fullName,
                    TeamId = teamId,
                    GoalsScored = 0
                };

                await _playerRepository.AddAsync(player);
                await _playerRepository.SaveAsync();
                return player.Id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create player: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }

        public async Task UpdatePlayerAsync(int id, int jerseyNumber, string fullName, int teamId)
        {
            if (jerseyNumber < 1 || jerseyNumber > 99)
                throw new ArgumentException("Jersey number must be between 1 and 99");
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > 100)
                throw new ArgumentException("Full name must be 1-100 characters");

            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                throw new KeyNotFoundException($"Player with ID {id} not found");

            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
                throw new KeyNotFoundException($"Team with ID {teamId} not found");

            if (!await _playerRepository.IsJerseyNumberUniqueInTeamAsync(teamId, jerseyNumber, id))
                throw new InvalidOperationException("Jersey number already exists in this team");

            player.JerseyNumber = jerseyNumber;
            player.FullName = fullName;
            player.TeamId = teamId;

            await _playerRepository.UpdateAsync(player);
            await _playerRepository.SaveAsync();
        }

        public async Task DeletePlayerAsync(int id)
        {
            var player = await _playerRepository.GetByIdAsync(id);
            if (player == null)
                throw new KeyNotFoundException($"Player with ID {id} not found");

            await _playerRepository.DeleteAsync(id);
            await _playerRepository.SaveAsync();
        }

        public async Task<IEnumerable<Player>> GetTopScorersAsync()
        {
            var players = await _playerRepository.GetAllAsync();
            return players
                .OrderByDescending(p => p.GoalsScored)
                .ThenBy(p => p.FullName);
        }
    }
}
