# Error: "An error occurred while saving the entity changes"

## Quick Fix Checklist

- [ ] **Are you trying to create a Team?**
  - Have you created a Stadium first?
  - Does the Stadium ID you're using actually exist?
  - Is the team name unique (not already used)?
  - Is the team code unique (1-99, not used before)?

- [ ] **Are you trying to create a Player?**
  - Have you created a Team first?
  - Does the Team ID you're using actually exist?
  - Is the jersey number unique for THIS team?

- [ ] **Are you trying to create a Match?**
  - Do both Team IDs exist?
  - Are the Team IDs different (can't play against itself)?
  - Are the goals non-negative numbers?

- [ ] **Are you trying to create a Stadium?**
  - Is the name unique?
  - Is the capacity > 0?

## Verification Steps

### Before creating data, verify what exists:

```
1. Go to "Stadium Management" → "View All Stadiums"
   (Note: Write down the Stadium IDs)

2. Go to "Team Management" → "View All Teams"
   (Note: Write down the Team IDs and confirm what's taken)

3. Go to "Player Management" → "View All Players"
   (Note: Verify jersey numbers per team)
```

## Most Common Cause

**You're referencing IDs that don't exist**

Example:
- You try to create a team with Stadium ID: 999
- But Stadium 999 doesn't exist
- Error: "Stadium with ID 999 not found"

**Solution**: Create Stadium first, check its ID, then use that ID

## Immediate Actions

1. **Stop the application** (Ctrl+C)
2. **Take note of the full error message** (including the "Details:" line)
3. **Run these commands**:

```powershell
# Remove the database
sqlcmd -S "(localdb)\mssqllocaldb" -Q "DROP DATABASE [FootballLeagueDb]"

# Rebuild clean
dotnet clean
dotnet build

# Run fresh
cd Football_League.UI
dotnet run
```

4. **Start fresh**:
   - Create 1 Stadium
   - Create 1 Team with that Stadium's ID
   - Create 1 Player with that Team's ID
   - Verify each step works before moving to next

## If Still Getting Errors

**The error message will now show you more details:**

```
❌ Error: [Main problem]
   Details: [Specific reason]
```

**Common detail messages**:
- "PRIMARY KEY constraint failed" → ID doesn't exist
- "UNIQUE constraint failed" → Name/code already used
- "FOREIGN KEY constraint failed" → Referenced entity missing
- "Unique constraint 'IX_..." → Something must be unique but isn't

## Database Status Check

Check if LocalDB is even running:

```powershell
sqllocaldb info
sqllocaldb start "mssqllocaldb"
```

## Final Nuclear Option

**If nothing works, completely reset everything:**

```powershell
# 1. Stop the app

# 2. Delete instance
sqllocaldb delete "mssqllocaldb"

# 3. Recreate instance
sqllocaldb create "mssqllocaldb"
sqllocaldb start "mssqllocaldb"

# 4. Clean and rebuild
cd C:\Users\Nergiz\source\repos\Football_League\
dotnet clean
dotnet build
cd Football_League.UI
dotnet run
```

The database will recreate automatically on first run.

## Example: Step-by-Step Working Workflow

**Step 1 - Create Stadium**
```
Option: 1 (Create Stadium)
Name: "Anfield"
Capacity: 61000
✓ Success: "Stadium created successfully with ID: 1"
```

**Step 2 - Create Team**
```
Option: 1 (Create Team)
Name: "Liverpool"
Code: 3
Stadium ID: 1  ← Use the ID from Step 1!
✓ Success: "Team created successfully with ID: 1"
```

**Step 3 - Create Player**
```
Option: 1 (Create Player)
Jersey: 7
Name: "Kenny Dalglish"
Team ID: 1  ← Use the Team ID from Step 2!
✓ Success: "Player created successfully with ID: 1"
```

**Step 4 - Verify**
```
Team Management → View All Teams → See "Liverpool"
Player Management → View Team Players → Enter 1 → See "Kenny Dalglish"
```

## Need More Help?

See **DATABASE_TROUBLESHOOTING.md** for detailed solutions to all scenarios.

## Remember

- **Always create parent entities first** (Stadium → Team → Player → Match)
- **Always use existing IDs** (check "View All" before using an ID)
- **Read error messages carefully** - they tell you what's wrong
- **The Details line** is your best friend - it shows the real reason
