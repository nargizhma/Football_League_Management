# Football League Management System

A complete n-tier console application for managing football leagues, teams, players, and matches.

## Project Structure

### 1. **Football_League.DA (Data Access Layer)** - Library
Handles all database operations using Entity Framework Core.

**Components:**
- **Entities**: Stadium, Team, Player, Match, MatchGoalscorer
- **DbContext**: FootballLeagueDbContext with Fluent API configuration
- **Repositories**: Interfaces and implementations for data access
  - IStadiumRepository / StadiumRepository
  - ITeamRepository / TeamRepository
  - IPlayerRepository / PlayerRepository
  - IMatchRepository / MatchRepository
  - IMatchGoalscorerRepository / MatchGoalscorerRepository

**Key Features:**
- Unique constraints on Team.Name and Team.Code
- Composite unique index on (TeamId, JerseyNumber) for Player
- Proper relationship configuration with appropriate delete behaviors
- Full async/await support

### 2. **Football_League.BLL (Business Logic Layer)** - Library
Contains all business rules and service implementations.

**Components:**
- **Services**: Interfaces and implementations
  - IStadiumService / StadiumService
  - ITeamService / TeamService
  - IPlayerService / PlayerService
  - IMatchService / MatchService
  - IReportService / ReportService

**Key Features:**
- Complete CRUD operations for all entities
- Team statistics management (Played, Wins, Draws, Losses, Points, etc.)
- Dynamic team statistics updates when matches are created/updated/deleted
- Player goal tracking
- Recalculation of standings from scratch when needed
- Validation of business rules
- Reports generation

### 3. **Football_League.UI (User Interface)** - Console Application
Menu-driven console interface for user interaction.

**Features:**
- Stadium Management (CRUD)
- Team Management (CRUD, Standings)
- Player Management (CRUD, Top Scorers)
- Match Management (CRUD with statistics updates)
- Reports (Standings, Top Scorers, Most Goals Scored/Conceded)

## Entity Relationships

```
Stadium (1) ──< (M) Team
Team (1) ──< (M) Player
Team (1) ──< (M) Match (HomeTeam)
Team (1) ──< (M) Match (AwayTeam)
Match (1) ──< (M) MatchGoalscorer
Player (1) ──< (M) MatchGoalscorer
```

## Team Statistics Management

When a match is created:
1. Both teams' `Played` count increments
2. `GoalsFor` and `GoalsAgainst` are updated
3. Win/Draw/Loss count is updated
4. Points are calculated (3 for win, 1 for draw, 0 for loss)
5. Player goal scores are updated

When a match is edited:
1. Team statistics are recalculated from scratch
2. Old data is cleared and rebuilt from all matches

When a match is deleted:
1. Team statistics are recalculated from scratch

## Database Setup

1. **Connection String**: Uses LocalDB by default (configured in `appsettings.json`)
2. **Database**: `FootballLeagueDb`
3. **Migrations**: Pre-configured initial migration in `Football_League.DA\Migrations`
4. **EF Core**: Automatically creates database on first run with `context.Database.MigrateAsync()`

## Running the Application

1. Ensure you have SQL Server LocalDB installed
2. Build the solution
3. Run `Football_League.UI`
4. The database will be created automatically on first run
5. Use the menu to navigate between different operations

## Validation Rules

**Stadium:**
- Name: 1-100 characters, required

**Team:**
- Name: 1-100 characters, unique, required
- Code: 1-99, unique, required
- All stats initialize to 0

**Player:**
- Jersey Number: 1-99, unique per team, required
- Full Name: 1-100 characters, required
- Goals Scored: Calculated from matches

**Match:**
- Home Team ≠ Away Team
- Week Number: ≥ 1
- Goals: ≥ 0

**MatchGoalscorer:**
- Player must belong to one of the match teams
- Goals Count: ≥ 1

## Report Features

1. **League Standings**: Sorted by Points → Goal Difference → Goals For → Team Name
2. **Team Statistics**: Individual team details with players list
3. **Top Scorers**: Players ranked by goals scored
4. **Most Goals Scored**: Teams ranked by goals for
5. **Most Goals Conceded**: Teams ranked by goals against

## Clean Code Principles Applied

- Meaningful names for all classes and methods
- Single responsibility principle for services and repositories
- Async/await throughout for scalability
- Business logic separated from data access
- Validation at service layer
- No magic numbers or hardcoded values
- Clear separation of concerns across layers
