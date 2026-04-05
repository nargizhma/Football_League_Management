using Microsoft.EntityFrameworkCore;
using Football_League.DAL.Data;
using Football_League.DAL.Repositories;
using Football_League.BLL.Services;

const string connectionString = "Data Source=localhost;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0";

var options = new DbContextOptionsBuilder<FootballLeagueDbContext>()
    .UseSqlServer(connectionString)
    .Options;

using var context = new FootballLeagueDbContext(options);
try
{
    // Ensure database is created and migrations are applied
    await context.Database.EnsureCreatedAsync();
    await context.Database.MigrateAsync();
    Console.WriteLine("Database initialized successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization error: {ex.Message}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    Console.WriteLine("Attempting to recreate database...");

    try
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        Console.WriteLine("Database recreated successfully.");
    }
    catch (Exception recreateEx)
    {
        Console.WriteLine($"Failed to recreate database: {recreateEx.Message}");
        Environment.Exit(1);
    }
}

var stadiumRepo = new StadiumRepository(context);
var teamRepo = new TeamRepository(context);
var playerRepo = new PlayerRepository(context);
var matchRepo = new MatchRepository(context);
var matchGoalRepo = new MatchGoalscorerRepository(context);

var stadiumService = new StadiumService(stadiumRepo);
var teamService = new TeamService(teamRepo, stadiumRepo);
var playerService = new PlayerService(playerRepo, teamRepo);
var matchService = new MatchService(matchRepo, teamRepo, playerRepo, matchGoalRepo);
var reportService = new ReportService(teamRepo, playerRepo);

var ui = new ConsoleUI(stadiumService, teamService, playerService, matchService, reportService);
await ui.RunAsync();

class ConsoleUI
{
    private readonly IStadiumService _stadiumService;
    private readonly ITeamService _teamService;
    private readonly IPlayerService _playerService;
    private readonly IMatchService _matchService;
    private readonly IReportService _reportService;

    public ConsoleUI(
        IStadiumService stadiumService,
        ITeamService teamService,
        IPlayerService playerService,
        IMatchService matchService,
        IReportService reportService)
    {
        _stadiumService = stadiumService;
        _teamService = teamService;
        _playerService = playerService;
        _matchService = matchService;
        _reportService = reportService;
    }

    public async Task RunAsync()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║          FOOTBALL LEAGUE MANAGEMENT SYSTEM                        ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. Stadium Management                                              ║");
            Console.WriteLine("║ 2. Team Management                                                 ║");
            Console.WriteLine("║ 3. Player Management                                               ║");
            Console.WriteLine("║ 4. Match Management                                                ║");
            Console.WriteLine("║ 5. Reports                                                         ║");
            Console.WriteLine("║ 6. Exit                                                            ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        await StadiumMenuAsync();
                        break;
                    case 2:
                        await TeamMenuAsync();
                        break;
                    case 3:
                        await PlayerMenuAsync();
                        break;
                    case 4:
                        await MatchMenuAsync();
                        break;
                    case 5:
                        await ReportsMenuAsync();
                        break;
                    case 6:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
            }
        }

        Console.WriteLine("\nThank you for using Football League Management System. Goodbye!");
    }

    private async Task StadiumMenuAsync()
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                     STADIUM MANAGEMENT                             ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. Create Stadium                                                  ║");
            Console.WriteLine("║ 2. View All Stadiums                                               ║");
            Console.WriteLine("║ 3. Update Stadium                                                  ║");
            Console.WriteLine("║ 4. Delete Stadium                                                  ║");
            Console.WriteLine("║ 5. Back to Main Menu                                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1:
                            await CreateStadiumAsync();
                            break;
                        case 2:
                            await ViewAllStadiumsAsync();
                            break;
                        case 3:
                            await UpdateStadiumAsync();
                            break;
                        case 4:
                            await DeleteStadiumAsync();
                            break;
                        case 5:
                            inMenu = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task CreateStadiumAsync()
    {
        Console.WriteLine("\n--- Create New Stadium ---");
        Console.Write("Enter stadium name: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Enter capacity: ");
        if (!int.TryParse(Console.ReadLine(), out int capacity))
        {
            Console.WriteLine("Invalid capacity. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        int id = await _stadiumService.CreateStadiumAsync(name, capacity);
        Console.WriteLine($"\nStadium created successfully with ID: {id}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewAllStadiumsAsync()
    {
        Console.Clear();
        Console.WriteLine("\n--- All Stadiums ---");
        var stadiums = await _stadiumService.GetAllStadiumsAsync();
        foreach (var stadium in stadiums)
        {
            Console.WriteLine($"ID: {stadium.Id}, Name: {stadium.Name}, Capacity: {stadium.Capacity}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task UpdateStadiumAsync()
    {
        Console.WriteLine("\n--- Update Stadium ---");
        Console.Write("Enter stadium ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var stadium = await _stadiumService.GetStadiumByIdAsync(id);
        if (stadium == null)
        {
            Console.WriteLine("Stadium not found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write($"Enter new name (current: {stadium.Name}): ");
        string name = Console.ReadLine() ?? stadium.Name;
        Console.Write($"Enter new capacity (current: {stadium.Capacity}): ");
        if (!int.TryParse(Console.ReadLine(), out int capacity))
            capacity = stadium.Capacity;

        await _stadiumService.UpdateStadiumAsync(id, name, capacity);
        Console.WriteLine("Stadium updated successfully. Press any key to continue...");
        Console.ReadKey();
    }

    private async Task DeleteStadiumAsync()
    {
        Console.WriteLine("\n--- Delete Stadium ---");
        Console.Write("Enter stadium ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Are you sure? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            await _stadiumService.DeleteStadiumAsync(id);
            Console.WriteLine("Stadium deleted successfully. Press any key to continue...");
        }
        else
        {
            Console.WriteLine("Deletion cancelled. Press any key to continue...");
        }
        Console.ReadKey();
    }

    private async Task TeamMenuAsync()
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                       TEAM MANAGEMENT                              ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. Create Team                                                     ║");
            Console.WriteLine("║ 2. View All Teams                                                  ║");
            Console.WriteLine("║ 3. View Team Details                                               ║");
            Console.WriteLine("║ 4. Update Team                                                     ║");
            Console.WriteLine("║ 5. Delete Team                                                     ║");
            Console.WriteLine("║ 6. Back to Main Menu                                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1:
                            await CreateTeamAsync();
                            break;
                        case 2:
                            await ViewAllTeamsAsync();
                            break;
                        case 3:
                            await ViewTeamDetailsAsync();
                            break;
                        case 4:
                            await UpdateTeamAsync();
                            break;
                        case 5:
                            await DeleteTeamAsync();
                            break;
                        case 6:
                            inMenu = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task CreateTeamAsync()
    {
        Console.WriteLine("\n--- Create New Team ---");
        Console.Write("Enter team name: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Enter team code (1-99): ");
        if (!int.TryParse(Console.ReadLine(), out int code))
        {
            Console.WriteLine("Invalid code. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter stadium ID: ");
        if (!int.TryParse(Console.ReadLine(), out int stadiumId))
        {
            Console.WriteLine("Invalid stadium ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        int teamId = await _teamService.CreateTeamAsync(name, code, stadiumId);
        Console.WriteLine($"\nTeam created successfully with ID: {teamId}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewAllTeamsAsync()
    {
        Console.Clear();
        Console.WriteLine("\n--- All Teams ---");
        var teams = await _teamService.GetAllTeamsAsync();
        foreach (var team in teams)
        {
            Console.WriteLine($"ID: {team.Id}, Name: {team.Name}, Code: {team.Code}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewTeamDetailsAsync()
    {
        Console.WriteLine("\n--- View Team Details ---");
        Console.Write("Enter team ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var report = await _reportService.GetTeamStatisticsReportAsync(id);
        Console.WriteLine(report);
        var playersReport = await _reportService.GetTeamPlayersReportAsync(id);
        Console.WriteLine(playersReport);
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task UpdateTeamAsync()
    {
        Console.WriteLine("\n--- Update Team ---");
        Console.Write("Enter team ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var team = await _teamService.GetTeamByIdAsync(id);
        if (team == null)
        {
            Console.WriteLine("Team not found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write($"Enter new name (current: {team.Name}): ");
        string name = Console.ReadLine() ?? team.Name;
        Console.Write($"Enter new code (current: {team.Code}): ");
        if (!int.TryParse(Console.ReadLine(), out int code))
            code = team.Code;
        Console.Write($"Enter new stadium ID (current: {team.StadiumId}): ");
        if (!int.TryParse(Console.ReadLine(), out int stadiumId))
            stadiumId = team.StadiumId;

        await _teamService.UpdateTeamAsync(id, name, code, stadiumId);
        Console.WriteLine("Team updated successfully. Press any key to continue...");
        Console.ReadKey();
    }

    private async Task DeleteTeamAsync()
    {
        Console.WriteLine("\n--- Delete Team ---");
        Console.Write("Enter team ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Are you sure? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            await _teamService.DeleteTeamAsync(id);
            Console.WriteLine("Team deleted successfully. Press any key to continue...");
        }
        else
        {
            Console.WriteLine("Deletion cancelled. Press any key to continue...");
        }
        Console.ReadKey();
    }

    private async Task PlayerMenuAsync()
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                      PLAYER MANAGEMENT                             ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. Create Player                                                   ║");
            Console.WriteLine("║ 2. View All Players                                                ║");
            Console.WriteLine("║ 3. View Team Players                                               ║");
            Console.WriteLine("║ 4. Update Player                                                   ║");
            Console.WriteLine("║ 5. Delete Player                                                   ║");
            Console.WriteLine("║ 6. Back to Main Menu                                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1:
                            await CreatePlayerAsync();
                            break;
                        case 2:
                            await ViewAllPlayersAsync();
                            break;
                        case 3:
                            await ViewTeamPlayersAsync();
                            break;
                        case 4:
                            await UpdatePlayerAsync();
                            break;
                        case 5:
                            await DeletePlayerAsync();
                            break;
                        case 6:
                            inMenu = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task CreatePlayerAsync()
    {
        Console.WriteLine("\n--- Create New Player ---");
        Console.Write("Enter jersey number (1-99): ");
        if (!int.TryParse(Console.ReadLine(), out int jerseyNumber))
        {
            Console.WriteLine("Invalid jersey number. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter full name: ");
        string fullName = Console.ReadLine() ?? "";
        Console.Write("Enter team ID: ");
        if (!int.TryParse(Console.ReadLine(), out int teamId))
        {
            Console.WriteLine("Invalid team ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        int playerId = await _playerService.CreatePlayerAsync(jerseyNumber, fullName, teamId);
        Console.WriteLine($"\nPlayer created successfully with ID: {playerId}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewAllPlayersAsync()
    {
        Console.Clear();
        Console.WriteLine("\n--- All Players ---");
        var players = await _playerService.GetAllPlayersAsync();
        foreach (var player in players)
        {
            Console.WriteLine($"ID: {player.Id}, Jersey: {player.JerseyNumber}, Name: {player.FullName}, Team ID: {player.TeamId}, Goals: {player.GoalsScored}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewTeamPlayersAsync()
    {
        Console.WriteLine("\n--- View Team Players ---");
        Console.Write("Enter team ID: ");
        if (!int.TryParse(Console.ReadLine(), out int teamId))
        {
            Console.WriteLine("Invalid team ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var players = await _playerService.GetPlayersByTeamIdAsync(teamId);
        foreach (var player in players)
        {
            Console.WriteLine($"Jersey: {player.JerseyNumber}, Name: {player.FullName}, Goals: {player.GoalsScored}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task UpdatePlayerAsync()
    {
        Console.WriteLine("\n--- Update Player ---");
        Console.Write("Enter player ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var player = await _playerService.GetPlayerByIdAsync(id);
        if (player == null)
        {
            Console.WriteLine("Player not found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write($"Enter new jersey number (current: {player.JerseyNumber}): ");
        if (!int.TryParse(Console.ReadLine(), out int jerseyNumber))
            jerseyNumber = player.JerseyNumber;
        Console.Write($"Enter new full name (current: {player.FullName}): ");
        string fullName = Console.ReadLine() ?? player.FullName;
        Console.Write($"Enter new team ID (current: {player.TeamId}): ");
        if (!int.TryParse(Console.ReadLine(), out int teamId))
            teamId = player.TeamId;

        await _playerService.UpdatePlayerAsync(id, jerseyNumber, fullName, teamId);
        Console.WriteLine("Player updated successfully. Press any key to continue...");
        Console.ReadKey();
    }

    private async Task DeletePlayerAsync()
    {
        Console.WriteLine("\n--- Delete Player ---");
        Console.Write("Enter player ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Are you sure? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            await _playerService.DeletePlayerAsync(id);
            Console.WriteLine("Player deleted successfully. Press any key to continue...");
        }
        else
        {
            Console.WriteLine("Deletion cancelled. Press any key to continue...");
        }
        Console.ReadKey();
    }

    private async Task MatchMenuAsync()
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                       MATCH MANAGEMENT                             ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. Create Match                                                    ║");
            Console.WriteLine("║ 2. View All Matches                                                ║");
            Console.WriteLine("║ 3. Update Match                                                    ║");
            Console.WriteLine("║ 4. Delete Match                                                    ║");
            Console.WriteLine("║ 5. Back to Main Menu                                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1:
                            await CreateMatchAsync();
                            break;
                        case 2:
                            await ViewAllMatchesAsync();
                            break;
                        case 3:
                            await UpdateMatchAsync();
                            break;
                        case 4:
                            await DeleteMatchAsync();
                            break;
                        case 5:
                            inMenu = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task CreateMatchAsync()
    {
        Console.WriteLine("\n--- Create New Match ---");
        Console.Write("Enter home team ID: ");
        if (!int.TryParse(Console.ReadLine(), out int homeTeamId))
        {
            Console.WriteLine("Invalid team ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter away team ID: ");
        if (!int.TryParse(Console.ReadLine(), out int awayTeamId))
        {
            Console.WriteLine("Invalid team ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter week number: ");
        if (!int.TryParse(Console.ReadLine(), out int weekNumber))
        {
            Console.WriteLine("Invalid week number. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter home team goals: ");
        if (!int.TryParse(Console.ReadLine(), out int homeGoals))
        {
            Console.WriteLine("Invalid goals. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Enter away team goals: ");
        if (!int.TryParse(Console.ReadLine(), out int awayGoals))
        {
            Console.WriteLine("Invalid goals. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        int matchId = await _matchService.CreateMatchAsync(homeTeamId, awayTeamId, weekNumber, homeGoals, awayGoals);
        Console.WriteLine($"\nMatch created successfully with ID: {matchId}");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewAllMatchesAsync()
    {
        Console.Clear();
        Console.WriteLine("\n--- All Matches ---");
        var matches = await _matchService.GetAllMatchesWithDetailsAsync();
        foreach (var match in matches)
        {
            Console.WriteLine($"ID: {match.Id}, Week {match.WeekNumber}: {match.HomeTeam?.Name} {match.HomeGoals}-{match.AwayGoals} {match.AwayTeam?.Name}");
        }
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task UpdateMatchAsync()
    {
        Console.WriteLine("\n--- Update Match ---");
        Console.Write("Enter match ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var match = await _matchService.GetMatchByIdAsync(id);
        if (match == null)
        {
            Console.WriteLine("Match not found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write($"Enter new home goals (current: {match.HomeGoals}): ");
        if (!int.TryParse(Console.ReadLine(), out int homeGoals))
            homeGoals = match.HomeGoals;
        Console.Write($"Enter new away goals (current: {match.AwayGoals}): ");
        if (!int.TryParse(Console.ReadLine(), out int awayGoals))
            awayGoals = match.AwayGoals;

        await _matchService.UpdateMatchAsync(id, homeGoals, awayGoals);
        Console.WriteLine("Match updated successfully. Press any key to continue...");
        Console.ReadKey();
    }

    private async Task DeleteMatchAsync()
    {
        Console.WriteLine("\n--- Delete Match ---");
        Console.Write("Enter match ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write("Are you sure? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            await _matchService.DeleteMatchAsync(id);
            Console.WriteLine("Match deleted successfully. Press any key to continue...");
        }
        else
        {
            Console.WriteLine("Deletion cancelled. Press any key to continue...");
        }
        Console.ReadKey();
    }

    private async Task ReportsMenuAsync()
    {
        bool inMenu = true;
        while (inMenu)
        {
            Console.Clear();
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                         REPORTS                                   ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. League Standings                                                ║");
            Console.WriteLine("║ 2. Top Scorers                                                     ║");
            Console.WriteLine("║ 3. Teams with Most Goals Scored                                    ║");
            Console.WriteLine("║ 4. Teams with Most Goals Conceded                                  ║");
            Console.WriteLine("║ 5. Back to Main Menu                                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝");
            Console.Write("\nSelect option: ");

            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                try
                {
                    switch (choice)
                    {
                        case 1:
                            Console.Clear();
                            var standings = await _reportService.GetTeamStandingsReportAsync();
                            Console.WriteLine(standings);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            break;
                        case 2:
                            Console.Clear();
                            var topScorers = await _reportService.GetTopScorersReportAsync();
                            Console.WriteLine(topScorers);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            break;
                        case 3:
                            Console.Clear();
                            var mostGoalsScored = await _reportService.GetMostGoalsScoredReportAsync();
                            Console.WriteLine(mostGoalsScored);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            break;
                        case 4:
                            Console.Clear();
                            var mostGoalsConceded = await _reportService.GetMostGoalsConcededReportAsync();
                            Console.WriteLine(mostGoalsConceded);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                            break;
                        case 5:
                            inMenu = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Press any key to continue...");
                            Console.ReadKey();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
}
