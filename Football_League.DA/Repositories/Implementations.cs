using Microsoft.EntityFrameworkCore;
using Football_League.DAL.Data;
using Football_League.DAL.Entities;

namespace Football_League.DAL.Repositories
{
    public class StadiumRepository : IStadiumRepository
    {
        private readonly FootballLeagueDbContext _context;

        public StadiumRepository(FootballLeagueDbContext context)
        {
            _context = context;
        }

        public async Task<Stadium?> GetByIdAsync(int id)
        {
            return await _context.Stadiums.FindAsync(id);
        }

        public async Task<IEnumerable<Stadium>> GetAllAsync()
        {
            return await _context.Stadiums.ToListAsync();
        }

        public async Task AddAsync(Stadium stadium)
        {
            await _context.Stadiums.AddAsync(stadium);
        }

        public async Task UpdateAsync(Stadium stadium)
        {
            _context.Stadiums.Update(stadium);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var stadium = await GetByIdAsync(id);
            if (stadium != null)
            {
                _context.Stadiums.Remove(stadium);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

    public class TeamRepository : ITeamRepository
    {
        private readonly FootballLeagueDbContext _context;

        public TeamRepository(FootballLeagueDbContext context)
        {
            _context = context;
        }

        public async Task<Team?> GetByIdAsync(int id)
        {
            return await _context.Teams.FindAsync(id);
        }

        public async Task<Team?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.Stadium)
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _context.Teams.ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetAllWithDetailsAsync()
        {
            return await _context.Teams
                .Include(t => t.Stadium)
                .Include(t => t.Players)
                .ToListAsync();
        }

        public async Task<bool> IsTeamNameUniqueAsync(string name, int? excludeId = null)
        {
            var query = _context.Teams.Where(t => t.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task<bool> IsTeamCodeUniqueAsync(int code, int? excludeId = null)
        {
            var query = _context.Teams.Where(t => t.Code == code);
            if (excludeId.HasValue)
            {
                query = query.Where(t => t.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task AddAsync(Team team)
        {
            await _context.Teams.AddAsync(team);
        }

        public async Task UpdateAsync(Team team)
        {
            _context.Teams.Update(team);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var team = await GetByIdAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

    public class PlayerRepository : IPlayerRepository
    {
        private readonly FootballLeagueDbContext _context;

        public PlayerRepository(FootballLeagueDbContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task<IEnumerable<Player>> GetByTeamIdAsync(int teamId)
        {
            return await _context.Players
                .Where(p => p.TeamId == teamId)
                .OrderBy(p => p.JerseyNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task<bool> IsJerseyNumberUniqueInTeamAsync(int teamId, int jerseyNumber, int? excludeId = null)
        {
            var query = _context.Players
                .Where(p => p.TeamId == teamId && p.JerseyNumber == jerseyNumber);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }

        public async Task AddAsync(Player player)
        {
            await _context.Players.AddAsync(player);
        }

        public async Task UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var player = await GetByIdAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

    public class MatchRepository : IMatchRepository
    {
        private readonly FootballLeagueDbContext _context;

        public MatchRepository(FootballLeagueDbContext context)
        {
            _context = context;
        }

        public async Task<Match?> GetByIdAsync(int id)
        {
            return await _context.Matches.FindAsync(id);
        }

        public async Task<Match?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.MatchGoalscorers)
                    .ThenInclude(mg => mg.Player)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Match>> GetAllAsync()
        {
            return await _context.Matches.ToListAsync();
        }

        public async Task<IEnumerable<Match>> GetAllWithDetailsAsync()
        {
            return await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.MatchGoalscorers)
                    .ThenInclude(mg => mg.Player)
                .ToListAsync();
        }

        public async Task AddAsync(Match match)
        {
            await _context.Matches.AddAsync(match);
        }

        public async Task UpdateAsync(Match match)
        {
            _context.Matches.Update(match);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var match = await GetByIdAsync(id);
            if (match != null)
            {
                _context.Matches.Remove(match);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

    public class MatchGoalscorerRepository : IMatchGoalscorerRepository
    {
        private readonly FootballLeagueDbContext _context;

        public MatchGoalscorerRepository(FootballLeagueDbContext context)
        {
            _context = context;
        }

        public async Task<MatchGoalscorer?> GetByIdAsync(int id)
        {
            return await _context.MatchGoalscorers.FindAsync(id);
        }

        public async Task<IEnumerable<MatchGoalscorer>> GetByMatchIdAsync(int matchId)
        {
            return await _context.MatchGoalscorers
                .Where(mg => mg.MatchId == matchId)
                .Include(mg => mg.Player)
                .ToListAsync();
        }

        public async Task<IEnumerable<MatchGoalscorer>> GetByPlayerIdAsync(int playerId)
        {
            return await _context.MatchGoalscorers
                .Where(mg => mg.PlayerId == playerId)
                .ToListAsync();
        }

        public async Task AddAsync(MatchGoalscorer matchGoalscorer)
        {
            await _context.MatchGoalscorers.AddAsync(matchGoalscorer);
        }

        public async Task UpdateAsync(MatchGoalscorer matchGoalscorer)
        {
            _context.MatchGoalscorers.Update(matchGoalscorer);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var matchGoalscorer = await GetByIdAsync(id);
            if (matchGoalscorer != null)
            {
                _context.MatchGoalscorers.Remove(matchGoalscorer);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
