# Quick Start Guide

## Prerequisites
- .NET 10 SDK
- SQL Server LocalDB
- Visual Studio 2022+ or Visual Studio Code

## Getting Started

### 1. Build the Project
```powershell
dotnet build
```

### 2. Run the Application
```powershell
cd Football_League.UI
dotnet run
```

### 3. First Time Setup
The application will automatically:
- Create the `FootballLeagueDb` database
- Run all migrations
- Display the main menu

## Sample Workflow

### Step 1: Create Stadiums
1. Select "Stadium Management"
2. Select "Create Stadium"
3. Enter stadium details:
   - Name: "Old Trafford"
   - Capacity: 75000

### Step 2: Create Teams
1. Select "Team Management"
2. Select "Create Team"
3. Enter team details:
   - Name: "Manchester United"
   - Code: 1
   - Stadium ID: 1 (from Step 1)

### Step 3: Create Players
1. Select "Player Management"
2. Select "Create Player"
3. Enter player details:
   - Jersey Number: 7
   - Full Name: "David Beckham"
   - Team ID: 1 (from Step 2)

### Step 4: Create Matches
1. Select "Match Management"
2. Select "Create Match"
3. Enter match details:
   - Home Team ID: 1
   - Away Team ID: 2
   - Week: 1
   - Home Goals: 3
   - Away Goals: 2

### Step 5: View Reports
1. Select "Reports"
2. View various reports:
   - League Standings
   - Top Scorers
   - Most Goals Scored
   - Most Goals Conceded

## Architecture Overview

### Three-Tier Architecture

```
┌─────────────────────────────────┐
│    UI Layer (Console App)       │  ← User Interface
├─────────────────────────────────┤
│    BLL (Business Logic)         │  ← Business Rules
│  - Services with validation     │
│  - Statistics calculation       │
├─────────────────────────────────┤
│    DAL (Data Access)            │  ← Database Operations
│  - Repositories                 │
│  - EF Core context              │
├─────────────────────────────────┤
│    Database (SQL Server)        │  ← Data Storage
└─────────────────────────────────┘
```

## Key Features

✅ **Complete CRUD Operations**
- Create, Read, Update, Delete for all entities

✅ **Dynamic Statistics**
- Automatic calculation of team standings
- Player goal tracking
- Win/Draw/Loss records

✅ **Data Validation**
- Unique team names and codes
- Unique jersey numbers per team
- Business rule enforcement

✅ **Comprehensive Reporting**
- League standings with proper sorting
- Top scorers list
- Team performance metrics

✅ **User-Friendly Interface**
- Menu-driven navigation
- Error handling and validation messages
- Formatted table output

## Data Persistence

- Database: SQL Server LocalDB
- Automatic migrations on startup
- All data persists between sessions

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server LocalDB is installed
- Check connection string in `appsettings.json`
- Verify database permissions

### Build Failures
- Clean solution: `dotnet clean`
- Restore packages: `dotnet restore`
- Rebuild: `dotnet build`

### Migration Issues
- Delete the database and restart (it will recreate)
- Ensure EF Core tools are installed: `dotnet tool install --global dotnet-ef`

## Development Notes

### Adding New Features
1. Add entity in `Football_League.DA\Entities`
2. Update `DbContext` with Fluent API configuration
3. Create migration
4. Implement repository in `Football_League.DA\Repositories`
5. Create service in `Football_League.BLL\Services`
6. Add UI menu in `Football_League.UI\Program.cs`

### Testing
- Services are designed to be testable
- Use async/await for scalability
- All business logic is in the service layer

## Contact & Support

For issues or improvements, refer to the README.md for detailed documentation.
