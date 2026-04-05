# Database Error Troubleshooting Guide

## Common Error: "An error occurred while saving the entity changes"

This error typically indicates that EF Core encountered a database constraint or configuration issue when trying to save data.

### Root Causes & Solutions

#### 1. **Foreign Key Constraint Violation**

**Symptom**: Error occurs when creating teams, players, or matches

**Solution**:
- When creating a team, ensure the Stadium ID exists
- When creating a player, ensure the Team ID exists
- When creating a match, ensure both Team IDs exist

**How to verify**:
1. Go to "Stadium Management"
2. Select "View All Stadiums"
3. Note the Stadium IDs
4. Use those IDs when creating teams

#### 2. **Unique Constraint Violation**

**Symptom**: Error when creating teams or players with duplicate names/codes

**Solution**:
- Team names must be unique
- Team codes (1-99) must be unique
- Jersey numbers must be unique per team (but can repeat across teams)

**How to verify**:
1. Go to "Team Management"
2. Select "View All Teams"
3. Check existing team names and codes
4. Use different values when creating new teams

#### 3. **Database Migration Issues**

**Symptom**: Error on startup about migrations

**Solution A - Let it auto-recover**:
- The application now has automatic database recreation logic
- It will attempt to delete and recreate the database if migration fails

**Solution B - Manual cleanup**:
```powershell
# Delete the database from SQL Server LocalDB
sqlcmd -S "(localdb)\mssqllocaldb"
DROP DATABASE [FootballLeagueDb]
GO
EXIT

# Then run the application again - it will recreate the database
```

**Solution C - Check LocalDB status**:
```powershell
# Verify LocalDB is running
sqllocaldb info

# If FootballLeagueDb instance doesn't exist, create it:
sqllocaldb create FootballLeagueDb

# Start the instance:
sqllocaldb start FootballLeagueDb
```

#### 4. **Connection String Issues**

**Current Connection String**:
```
Server=(localdb)\mssqllocaldb;Database=FootballLeagueDb;Trusted_Connection=true;
```

**If you get connection errors**:

Option 1 - Use instance name:
```
Server=(localdb)\mssqllocaldb;Database=FootballLeagueDb;Trusted_Connection=true;
```

Option 2 - Use SQL Server 2019 LocalDB:
```
Server=(localdb)\MSSQLLocalDB;Database=FootballLeagueDb;Trusted_Connection=true;
```

Option 3 - Use specific instance:
```
Server=.\SQLEXPRESS;Database=FootballLeagueDb;Trusted_Connection=true;
```

**To update connection string**:
Edit `Football_League.UI\Program.cs` at the top:
```csharp
const string connectionString = "YOUR_CONNECTION_STRING_HERE";
```

#### 5. **NULL Reference or Type Mismatch**

**Symptom**: Error mentioning null values or type mismatch

**Solutions**:
- Ensure all required fields are filled in the console prompts
- Check that numeric inputs are actually numbers
- Verify all foreign keys reference existing records

#### 6. **Relationship Cascading Issues**

**Symptom**: Error when deleting teams that have players or matches

**Expected behavior**:
- Players are automatically deleted when team is deleted (cascade delete)
- Matches cannot be deleted if teams are still involved (restrict delete)
- MatchGoalscorers are automatically deleted when matches are deleted

### Diagnostic Steps

**Step 1 - Check database existence**:
1. Run the application
2. Look for console output about database initialization
3. Note any error messages

**Step 2 - View all data before adding new data**:
1. Go to "Stadium Management" → "View All Stadiums"
2. Go to "Team Management" → "View All Teams"
3. Go to "Player Management" → "View All Players"
4. This helps you understand existing data and IDs

**Step 3 - Create minimal data**:
1. Create 1 stadium
2. Create 1 team (using the stadium ID)
3. Create 1 player (using the team ID)
4. If these work, try creating a match

**Step 4 - Check error messages carefully**:
The new error handling will show:
- Main error message (what went wrong)
- Inner exception details (why it went wrong)

### Advanced Debugging

If errors persist, check:

1. **SQL Server LocalDB Status**:
```powershell
sqllocaldb info
sqllocaldb start "mssqllocaldb"
```

2. **Database Contents** (using SSMS or VS):
- Connect to `(localdb)\mssqllocaldb`
- Look for database `FootballLeagueDb`
- Check tables and their contents

3. **Recent Changes**:
- If you modified entity relationships in the code
- You may need to delete and recreate the database
- Delete from `Football_League.DA\Migrations` folder and regenerate

### Quick Recovery Steps

If everything fails:

```powershell
# 1. Close the application

# 2. Delete the database
sqlcmd -S "(localdb)\mssqllocaldb" -Q "DROP DATABASE [FootballLeagueDb]"

# 3. Restart LocalDB
sqllocaldb stop "mssqllocaldb"
sqllocaldb start "mssqllocaldb"

# 4. Rebuild the solution
dotnet clean
dotnet build

# 5. Run the application
cd Football_League.UI
dotnet run
```

### Example Workflow That Works

1. **Create Stadium**:
   - Name: "Old Trafford"
   - Capacity: 75000
   - ID: 1 (note this)

2. **Create Team**:
   - Name: "Manchester United"
   - Code: 1
   - Stadium ID: 1
   - ID: 1 (note this)

3. **Create Player**:
   - Jersey: 7
   - Name: "David Beckham"
   - Team ID: 1

4. **View to verify**:
   - Check team list
   - Check team players
   - View standings (should show 0-0)

### Getting Detailed Error Information

The application now shows:
```
❌ Error: [Main error message]
   Details: [Inner exception details]
```

**Take note of these details when reporting issues**:
- What action caused the error?
- What values did you enter?
- What are the error and details messages?

### Common Error Messages & Meanings

| Error | Meaning | Solution |
|-------|---------|----------|
| "Stadium with ID X not found" | Stadium doesn't exist | Create stadium first or check ID |
| "Team name must be unique" | Name already exists | Use different team name |
| "Team code must be unique" | Code already exists | Use different code (1-99) |
| "Home team and away team cannot be the same" | Same team in both positions | Select different teams |
| "Foreign key constraint" | Referenced entity missing | Create the related entity first |
| "Unique constraint violation" | Duplicate data | Check for duplicates |

### Still Having Issues?

1. **Collect error details**: Screenshot the error message
2. **Describe the steps**: What were you doing when it failed?
3. **Note the values**: What data did you enter?
4. **Check this guide**: Refer to the relevant section above

The application is designed to be resilient, but database issues can occur. Following these steps should resolve most problems.
