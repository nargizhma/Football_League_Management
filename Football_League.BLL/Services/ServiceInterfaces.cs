using Football_League.DAL.Entities;

namespace Football_League.BLL.Services
{
    public interface IStadiumService
    {
        Task<Stadium?> GetStadiumByIdAsync(int id);
        Task<IEnumerable<Stadium>> GetAllStadiumsAsync();
        Task<int> CreateStadiumAsync(string name, int capacity);
        Task UpdateStadiumAsync(int id, string name, int capacity);
        Task DeleteStadiumAsync(int id);
    }

    public interface ITeamService
    {
        Task<Team?> GetTeamByIdAsync(int id);
        Task<Team?> GetTeamByIdWithDetailsAsync(int id);
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<IEnumerable<Team>> GetAllTeamsWithDetailsAsync();
        Task<int> CreateTeamAsync(string name, int code, int stadiumId);
        Task UpdateTeamAsync(int id, string name, int code, int stadiumId);
        Task DeleteTeamAsync(int id);
        Task<IEnumerable<Team>> GetStandingsAsync();
    }

    public interface IPlayerService
    {
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<IEnumerable<Player>> GetPlayersByTeamIdAsync(int teamId);
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task<int> CreatePlayerAsync(int jerseyNumber, string fullName, int teamId);
        Task UpdatePlayerAsync(int id, int jerseyNumber, string fullName, int teamId);
        Task DeletePlayerAsync(int id);
        Task<IEnumerable<Player>> GetTopScorersAsync();
    }

    public interface IMatchService
    {
        Task<Match?> GetMatchByIdAsync(int id);
        Task<Match?> GetMatchByIdWithDetailsAsync(int id);
        Task<IEnumerable<Match>> GetAllMatchesAsync();
        Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync();
        Task<int> CreateMatchAsync(int homeTeamId, int awayTeamId, int weekNumber, int homeGoals, int awayGoals, IEnumerable<(int PlayerId, int Goals)>? goalscorers = null);
        Task UpdateMatchAsync(int id, int homeGoals, int awayGoals, IEnumerable<(int PlayerId, int Goals)>? goalscorers = null);
        Task DeleteMatchAsync(int id);
    }

    public interface IReportService
    {
        Task<string> GetTeamStandingsReportAsync();
        Task<string> GetTeamStatisticsReportAsync(int teamId);
        Task<string> GetTeamPlayersReportAsync(int teamId);
        Task<string> GetTopScorersReportAsync();
        Task<string> GetMostGoalsConcededReportAsync();
        Task<string> GetMostGoalsScoredReportAsync();
    }
}
