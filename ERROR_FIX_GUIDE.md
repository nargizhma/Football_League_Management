# How to Fix: "An error occurred while saving the entity changes"

## What Does This Error Mean?

This error happens when Entity Framework Core tried to save data to the database but something prevented it. This is almost always caused by:

1. **Missing Foreign Key** - You referenced an entity (Stadium, Team, etc.) that doesn't exist
2. **Unique Constraint Violation** - You tried to create duplicate data that must be unique
3. **Database Connection Issue** - Can't reach the database
4. **Migration Issue** - Database tables aren't set up correctly

## Step 1: Identify When It Happens

**What were you doing when the error occurred?**

- [ ] Creating a Stadium?
- [ ] Creating a Team?
- [ ] Creating a Player?
- [ ] Creating a Match?
- [ ] Creating something else?

Write this down - it helps find the real problem.

## Step 2: The New Enhanced Error Messages

The application has been updated to show **TWO** error messages:

```
❌ Error: [Main message]
   Details: [Inner exception message]
```

**Example:**
```
❌ Error: Failed to create team: Could not insert or update a row because a foreign key constraint would be violated
   Details: The INSERT, UPDATE, or DELETE statement conflicted with a FOREIGN KEY constraint "FK_Teams_Stadiums_StadiumId"
```

**Translation:** Stadium doesn't exist. Use a valid Stadium ID.

## Step 3: Common Error Scenarios & Fixes

### Scenario 1: Creating Teams Fails

**Error message might say:**
```
Error: Failed to create team: [database constraint error]
Details: FOREIGN KEY constraint...StadiumId
```

**What went wrong:** Stadium ID doesn't exist

**How to fix:**
```
1. Go: Team Management → Back
2. Go: Stadium Management → View All Stadiums
3. Write down a Stadium ID (e.g., 1)
4. Go back to Team Management → Create Team
5. Use the Stadium ID you wrote down
```

### Scenario 2: Creating Players Fails

**Error message might say:**
```
Error: Failed to create player: [database constraint error]
Details: FOREIGN KEY constraint...TeamId
```

**What went wrong:** Team ID doesn't exist

**How to fix:**
```
1. Go: Player Management → Back
2. Go: Team Management → View All Teams
3. Write down a Team ID (e.g., 1)
4. Go back to Player Management → Create Player
5. Use the Team ID you wrote down
```

### Scenario 3: Team Already Exists

**Error message might say:**
```
Error: Team name must be unique
Details: [database error about unique constraint]
```

**What went wrong:** You already created a team with that name

**How to fix:**
```
1. Use a different team name
2. Or delete the existing team first (Team Management → Delete Team)
```

### Scenario 4: Matches Won't Create

**Error message might say:**
```
Error: Failed to create match: [constraint error]
Details: FOREIGN KEY constraint...HomeTeamId or AwayTeamId
```

**What went wrong:** Team ID doesn't exist

**How to fix:**
```
1. Go: Match Management → Back
2. Go: Team Management → View All Teams
3. Write down two different Team IDs
4. Go back to Match Management → Create Match
5. Use those Team IDs
6. Make sure they're DIFFERENT (can't play against itself)
```

## Step 4: The Most Common Cause

**You're using IDs that don't exist yet.**

```
Wrong:
- Create Team with Stadium ID: 999 (but Stadium 999 doesn't exist)
- Result: Foreign Key Error

Right:
- Create Stadium first → Get its ID (e.g., 1)
- Create Team with Stadium ID: 1
- Result: Success!
```

## Step 5: Manual Data Creation Workflow

**Always follow this order:**

```
1. Stadiums
   ↓
2. Teams (need Stadium IDs)
   ↓
3. Players (need Team IDs)
   ↓
4. Matches (need Team IDs)
```

### Example:

```
STEP 1 - CREATE STADIUM
Go to: Stadium Management → Create Stadium
Enter: Name="Anfield", Capacity=61000
✓ Get ID: 1

STEP 2 - CREATE TEAM
Go to: Team Management → Create Team
Enter: Name="Liverpool", Code=3, Stadium ID=1 ← USE ID FROM STEP 1
✓ Get ID: 1

STEP 3 - CREATE PLAYER
Go to: Player Management → Create Player
Enter: Jersey=7, Name="Kenny Dalglish", Team ID=1 ← USE ID FROM STEP 2
✓ Get ID: 1

STEP 4 - CREATE MATCH
Go to: Match Management → Create Match
Enter: Home Team ID=1, Away Team ID=(another team), etc.
✓ Success!
```

## Step 6: Verification Method

**Before creating new data, always check what exists:**

```
Command: Go to [Menu] → View All [Items]

Creates:
- Stadium Management → View All Stadiums (see all Stadium IDs)
- Team Management → View All Teams (see all Team IDs)
- Player Management → View All Players (see all Player IDs)
- Match Management → View All Matches (see all Match IDs)
```

## Step 7: If You Keep Getting Errors

Try these in order:

### Option 1 - Clean Delete and Rebuild

```powershell
# Stop the application (Ctrl+C)

# Delete the database
sqlcmd -S "(localdb)\mssqllocaldb" -Q "DROP DATABASE [FootballLeagueDb]"

# Clean and rebuild
cd C:\Users\Nergiz\source\repos\Football_League\
dotnet clean
dotnet build

# Run again
cd Football_League.UI
dotnet run
```

The database will auto-create on startup.

### Option 2 - Check LocalDB Status

```powershell
# See if LocalDB is running
sqllocaldb info

# If not running:
sqllocaldb start "mssqllocaldb"

# Verify connection
sqlcmd -S "(localdb)\mssqllocaldb"
GO
SELECT name FROM sys.databases WHERE name = 'FootballLeagueDb'
GO
EXIT
```

## Step 8: Foolproof Starter Kit

**Copy and use exactly these steps (guaranteed to work):**

1. Run the application
2. Go to Stadium Management
3. Select: 1. Create Stadium
4. Enter: Name = "Test Stadium" → Capacity = 50000
5. Write down the ID shown
6. Select: 5. Back to Main Menu
7. Go to Team Management
8. Select: 1. Create Team
9. Enter: Name = "Test Team" → Code = 1 → Stadium ID = (the ID from step 5)
10. Success!

If this works, you know the database is fine. The error must be in your input data.

## Step 9: Debug Checklist

When you get an error, check:

- [ ] Did you create the parent entity first? (Stadium before Team, Team before Player)
- [ ] Did you use the correct ID? (Check "View All" to confirm)
- [ ] Are you using unique values? (Team names, Team codes, Jersey numbers per team)
- [ ] Is the data being asked for correct type? (Numbers for IDs and codes, text for names)
- [ ] Did the application start without errors?
- [ ] Is the database file corrupted? (Try Option 1 above)

## Step 10: Getting Help

If you still have issues, provide:

1. **The exact error message** (screenshot or copy-paste the whole thing)
2. **What you were trying to do** (create stadium, team, etc.)
3. **What data you entered** (values you typed in)
4. **What "View All" shows** (do parent entities exist?)

With this info, the problem can be solved quickly.

## Quick Reference: Entity Dependencies

```
Stadium ← Required by → Team
Team ← Required by → Player
Team ← Required by → Match
Match ← Required by → MatchGoalscorer
Player ← Required by → MatchGoalscorer
```

**Translation:**
- You CANNOT create a Team without a Stadium existing first
- You CANNOT create a Player without a Team existing first
- You CANNOT create a Match without Teams existing first
- You CANNOT record a Goalscorer without Match and Player existing first

## Remember

The error "An error occurred while saving the entity changes" is **very fixable**. It almost always means:

1. **Missing parent entity** - Create the parent first
2. **Duplicate data** - Use different values
3. **Wrong ID reference** - Check "View All" and use correct IDs
4. **Database issue** - Clean delete and rebuild

Follow the steps above and your error will be resolved!
