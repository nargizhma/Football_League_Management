using Football_League.DAL.Entities;

namespace Football_League.DAL.Repositories
{
    public interface IStadiumRepository
    {
        Task<Stadium?> GetByIdAsync(int id);
        Task<IEnumerable<Stadium>> GetAllAsync();
        Task AddAsync(Stadium stadium);
        Task UpdateAsync(Stadium stadium);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }

    public interface ITeamRepository
    {
        Task<Team?> GetByIdAsync(int id);
        Task<Team?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Team>> GetAllAsync();
        Task<IEnumerable<Team>> GetAllWithDetailsAsync();
        Task<bool> IsTeamNameUniqueAsync(string name, int? excludeId = null);
        Task<bool> IsTeamCodeUniqueAsync(int code, int? excludeId = null);
        Task AddAsync(Team team);
        Task UpdateAsync(Team team);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }

    public interface IPlayerRepository
    {
        Task<Player?> GetByIdAsync(int id);
        Task<IEnumerable<Player>> GetByTeamIdAsync(int teamId);
        Task<IEnumerable<Player>> GetAllAsync();
        Task<bool> IsJerseyNumberUniqueInTeamAsync(int teamId, int jerseyNumber, int? excludeId = null);
        Task AddAsync(Player player);
        Task UpdateAsync(Player player);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }

    public interface IMatchRepository
    {
        Task<Match?> GetByIdAsync(int id);
        Task<Match?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Match>> GetAllAsync();
        Task<IEnumerable<Match>> GetAllWithDetailsAsync();
        Task AddAsync(Match match);
        Task UpdateAsync(Match match);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }

    public interface IMatchGoalscorerRepository
    {
        Task<MatchGoalscorer?> GetByIdAsync(int id);
        Task<IEnumerable<MatchGoalscorer>> GetByMatchIdAsync(int matchId);
        Task<IEnumerable<MatchGoalscorer>> GetByPlayerIdAsync(int playerId);
        Task AddAsync(MatchGoalscorer matchGoalscorer);
        Task UpdateAsync(MatchGoalscorer matchGoalscorer);
        Task DeleteAsync(int id);
        Task SaveAsync();
    }
}
