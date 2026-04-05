# Architecture & Design Documentation

## System Architecture

### Three-Tier Architecture Diagram

```
┌────────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER (UI)                    │
│                   Football_League.UI Console                   │
│                    - User Input/Output                         │
│                    - Menu Navigation                           │
│                    - Data Formatting                           │
└─────────────────────────┬──────────────────────────────────────┘
                          │
        ┌─────────────────┴─────────────────┐
        │                                   │
┌───────▼──────────────────────────────────▼──────┐
│        BUSINESS LOGIC LAYER (Services)         │
│           Football_League.BLL                  │
│  - StadiumService                              │
│  - TeamService                                 │
│  - PlayerService                               │
│  - MatchService                                │
│  - ReportService                               │
│  ─────────────────────────────────────         │
│  - Business Rules Enforcement                  │
│  - Statistics Calculation                      │
│  - Validation Logic                            │
└───────┬──────────────────────────────────────┬─┘
        │                                       │
┌───────▼───────────────────────────────────────▼──────┐
│        DATA ACCESS LAYER (Repositories)             │
│           Football_League.DA                        │
│  - StadiumRepository                               │
│  - TeamRepository                                  │
│  - PlayerRepository                                │
│  - MatchRepository                                 │
│  - MatchGoalscorerRepository                       │
│  ────────────────────────────────────              │
│  - DbContext Configuration                         │
│  - Entity Mapping (Fluent API)                     │
│  - Query Execution                                 │
└───────┬──────────────────────────────────────────┬──┘
        │                                          │
┌───────▼──────────────────────────────────────────▼───┐
│    DATABASE LAYER (SQL Server LocalDB)              │
│         FootballLeagueDb Database                    │
│  ─────────────────────────────────────              │
│  - Stadiums                                         │
│  - Teams                                            │
│  - Players                                          │
│  - Matches                                          │
│  - MatchGoalscorers                                 │
└────────────────────────────────────────────────────┘
```

## Entity Relationship Diagram

```
┌──────────────┐
│   Stadium    │
├──────────────┤
│ Id (PK)      │
│ Name         │
│ Capacity     │
└──────┬───────┘
       │ 1:M
       │
       ▼
┌──────────────────────────┐
│         Team             │
├──────────────────────────┤
│ Id (PK)                  │
│ Name (UNIQUE)            │
│ Code (UNIQUE)            │◄─┐
│ StadiumId (FK)           │  │
│ Played                   │  │
│ Wins                     │  │
│ Draws                    │  │
│ Losses                   │  │
│ GoalsFor                 │  │
│ GoalsAgainst             │  │
│ Points                   │  │
└──────┬──────────┬────────┘  │
       │ 1:M      │ 1:M       │
       │          │           │
    ┌──▼──┐  ┌────▼────┐      │
    │     │  │         │      │
    ▼     ▼  ▼         ▼      │
┌──────────┐  ┌──────────┐    │
│ Player   │  │  Match   │    │
├──────────┤  ├──────────┤    │
│ Id (PK)  │  │ Id (PK)  │    │
│ Jersey#  │  │ Home TID ├────┘ (FK)
│ FullName │  │ Away TID ├────┐ (FK)
│ TeamId   │  │ WeekNum  │    │
│ GoalsScrd│  │ HomeGoals│    │
└─────┬────┘  │ AwayGoals│    │
      │ 1:M   └────┬─────┘    │
      │            │          │
      │            ▼          │
      │      (Constraint)     │
      │      HomeTeam ≠       │
      │      AwayTeam         │
      │                       │
      │       1:M             │
      └──────────┬────────────┘
                 │ 1:M
                 ▼
        ┌────────────────────┐
        │ MatchGoalscorer    │
        ├────────────────────┤
        │ Id (PK)            │
        │ MatchId (FK)       │
        │ PlayerId (FK)      │
        │ GoalsCount         │
        └────────────────────┘
```

## Service Layer Interaction

```
┌─────────────────────────────────────────────────────┐
│              ConsoleUI (Program.cs)                 │
│  - Handles user input                               │
│  - Calls services                                   │
│  - Displays output                                  │
└────┬──────────────────────────────────────────────┬─┘
     │                                              │
┌────▼──────────────┐  ┌──────────────────────────▼──┐
│ StadiumService    │  │ TeamService                  │
│ PlayerService     │  │ MatchService                 │
└────┬──────────────┘  │ ReportService                │
     │                 └──────────────┬───────────────┘
     │                                │
     │    ┌────────────────────────────┘
     │    │
     ▼    ▼
┌──────────────────────────────────────────────────┐
│           Repository Interfaces                  │
│  ┌───────────────────────────────────────────┐  │
│  │ IStadiumRepository                        │  │
│  │ ITeamRepository                           │  │
│  │ IPlayerRepository                         │  │
│  │ IMatchRepository                          │  │
│  │ IMatchGoalscorerRepository                │  │
│  └───────────────────────────────────────────┘  │
└─────────┬──────────────────────────────────────┘
          │
          ▼
┌──────────────────────────────────────────────────┐
│      Repository Implementations                  │
│  Using DbContext for database operations         │
└──────────────────────────────────────────────────┘
```

## Data Flow Example: Creating a Match

```
User Input (Console)
        │
        ▼
┌───────────────────────────┐
│ MatchMenuAsync()          │ (UI Layer)
│ - Get teams               │
│ - Get goals               │
└────────┬──────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ MatchService.CreateMatchAsync()  │ (BLL Layer)
│ - Validate teams               │
│ - Validate goals               │
│ - Call Update Team Stats       │
└────────┬───────────────────────┘
         │
         ├─────┬─────────────────────────┐
         │     │                         │
         ▼     ▼                         ▼
    ┌─────────────────┐  ┌──────────────────────┐
    │ MatchRepository │  │ TeamRepository       │
    │ AddAsync()      │  │ UpdateAsync()        │
    │ SaveAsync()     │  │ SaveAsync()          │
    └─────────┬───────┘  └────────┬─────────────┘
              │                   │
              └───────┬───────────┘
                      │
                      ▼
            ┌──────────────────┐
            │  DbContext       │
            │  SaveChangesAsync│ (DAL Layer)
            └────────┬─────────┘
                     │
                     ▼
            ┌──────────────────┐
            │  SQL Server      │
            │  Database        │ (Database Layer)
            └──────────────────┘
                     │
                     ▼
        ┌────────────────────────┐
        │ Match Created          │
        │ Team Stats Updated     │
        │ Return Success Message │
        └────────────────────────┘
```

## Statistics Recalculation Flow

```
Match Operation (Create/Update/Delete)
        │
        ▼
┌──────────────────────────────────┐
│ MatchService operation begins    │
└────────┬─────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ RecalculateTeamStatisticsAsync() │
│ - Reset stats to 0               │
│ - Get all matches                │
└────────┬─────────────────────────┘
         │
         ├─────────────────────────────┐
         │ For each match involving   │
         │ either team:               │
         │                            │
         ▼                            │
┌─────────────────────────┐          │
│ Update Team Stats:      │          │
│ - Played++              │◄─────────┘
│ - GoalsFor += HG        │
│ - GoalsAgainst += AG    │
│ - Check result          │
│ - Award points          │
│ - Update W/D/L          │
└────────┬────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ Persist to Repository            │
│ Save changes to database         │
└──────────────────────────────────┘
         │
         ▼
┌──────────────────────────────────┐
│ Statistics Accurate & Consistent │
└──────────────────────────────────┘
```

## Class Responsibilities

### Data Layer
- **Entities**: Model definition only
- **DbContext**: EF Core configuration via Fluent API
- **Repositories**: CRUD operations, query execution
- **Result**: Database persistence

### Business Layer
- **Services**: Business logic, validation, coordination
- **Interfaces**: Contract definition
- **Purpose**: Enforce rules, calculate statistics
- **Result**: Business-validated data

### UI Layer
- **ConsoleUI**: Menu presentation, user interaction
- **Data Formatting**: Output formatting
- **Input Handling**: User input validation (basic)
- **Result**: User-friendly interface

## Design Patterns Used

1. **Repository Pattern**
   - Abstraction of data access
   - Testability through interfaces
   - Consistent CRUD operations

2. **Service Layer Pattern**
   - Business logic separation
   - Reusable across UI layers
   - Validation enforcement

3. **Dependency Injection**
   - Services injected via constructor
   - Loose coupling between layers
   - Easy to mock for testing

4. **Async/Await Pattern**
   - Non-blocking operations
   - Scalability
   - Responsiveness

## Validation Layers

```
User Input
    │
    ▼
┌─────────────┐
│  UI Parsing │ (Basic type checking)
└────┬────────┘
     │
     ▼
┌───────────────────────────┐
│  Service Validation       │ (Business rules)
│ - Uniqueness checks       │
│ - Range validation        │
│ - Relationship checks     │
└────┬───────────────────────┘
     │
     ▼
┌────────────────────┐
│  Database Level    │ (Constraints)
│ - Unique indexes   │
│ - Foreign keys     │
│ - Composite keys   │
└────────────────────┘
```

## Performance Considerations

1. **Async Operations**: All DB operations are async
2. **Eager Loading**: Include related data when needed
3. **Query Optimization**: Select only needed columns
4. **Statistics Recalculation**: O(n) where n = matches (acceptable for current scale)
5. **Caching**: Not implemented (could be added for read-heavy operations)

## Security Considerations

1. **Input Validation**: All user inputs validated
2. **SQL Injection**: EF Core parameterized queries
3. **Access Control**: Not implemented (single-user console app)
4. **Authentication**: Not required (local app)
5. **Data Encryption**: Not required (local DB)

## Scalability Recommendations

1. Add caching layer (Redis)
2. Implement pagination for large datasets
3. Add database indexes for frequent queries
4. Consider CQRS for complex reporting
5. Implement search and filtering
6. Add audit logging
7. Implement soft deletes for audit trail
