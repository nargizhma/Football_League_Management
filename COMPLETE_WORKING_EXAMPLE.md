# Complete Working Example - Step By Step

Follow these exact steps to get a fully working Football League with data.

## Prerequisites
- Application is running
- Main menu is displaying
- No errors on startup

## Complete Setup (Takes ~5 minutes)

### Phase 1: Create Stadiums

Go to main menu → Select **1. Stadium Management**

```
1. Create Stadium

Enter stadium name: Anfield
Enter capacity: 61000

✓ Success: "Stadium created successfully with ID: 1"
```

**Repeat for more stadiums** (select option 1 each time):

```
Stadium 2:
Name: Old Trafford
Capacity: 75000
✓ ID: 2

Stadium 3:
Name: Stamford Bridge
Capacity: 62000
✓ ID: 3

Stadium 4:
Name: Emirates Stadium
Capacity: 60000
✓ ID: 4
```

Then select option **5. Back to Main Menu**

### Phase 2: Create Teams

Go to main menu → Select **2. Team Management**

```
1. Create Team

Enter team name: Liverpool
Enter team code (1-99): 3
Enter stadium ID: 1

✓ Success: "Team created successfully with ID: 1"
```

**Repeat for more teams** (select option 1 each time):

```
Team 2:
Name: Manchester United
Code: 1
Stadium ID: 2
✓ ID: 2

Team 3:
Name: Chelsea
Code: 2
Stadium ID: 3
✓ ID: 3

Team 4:
Name: Arsenal
Code: 4
Stadium ID: 4
✓ ID: 4
```

Then select option **6. Back to Main Menu**

### Phase 3: Create Players

Go to main menu → Select **3. Player Management**

**Liverpool Players (Team ID 1)**

```
1. Create Player

Enter jersey number (1-99): 1
Enter full name: Ray Clemence
Enter team ID: 1

✓ Success: "Player created successfully with ID: 1"
```

Repeat these for Liverpool:
```
Jersey 7, Name: Kenny Dalglish, Team: 1 → ID: 2
Jersey 9, Name: Robbie Fowler, Team: 1 → ID: 3
```

**Manchester United Players (Team ID 2)**

```
Jersey 1, Name: Edwin van der Sar, Team: 2 → ID: 4
Jersey 7, Name: David Beckham, Team: 2 → ID: 5
Jersey 9, Name: Wayne Rooney, Team: 2 → ID: 6
```

**Chelsea Players (Team ID 3)**

```
Jersey 1, Name: Petr Cech, Team: 3 → ID: 7
Jersey 11, Name: Frank Lampard, Team: 3 → ID: 8
Jersey 15, Name: Didier Drogba, Team: 3 → ID: 9
```

**Arsenal Players (Team ID 4)**

```
Jersey 1, Name: Edwin Kraft, Team: 4 → ID: 10
Jersey 4, Name: Patrick Vieira, Team: 4 → ID: 11
Jersey 14, Name: Thierry Henry, Team: 4 → ID: 12
```

Then select option **6. Back to Main Menu**

### Phase 4: Create Matches

Go to main menu → Select **4. Match Management**

**Week 1 Matches**

```
1. Create Match

Enter home team ID: 1 (Liverpool)
Enter away team ID: 2 (Manchester United)
Enter week number: 1
Enter home team goals: 2
Enter away team goals: 1

✓ Success: "Match created successfully with ID: 1"
```

Create second Week 1 match:
```
Home Team: 3 (Chelsea)
Away Team: 4 (Arsenal)
Week: 1
Home Goals: 1
Away Goals: 0

✓ Success: ID: 2
```

**Week 2 Matches**

```
Home Team: 2 (Manchester United)
Away Team: 3 (Chelsea)
Week: 2
Home Goals: 3
Away Goals: 1

✓ Success: ID: 3

---

Home Team: 4 (Arsenal)
Away Team: 1 (Liverpool)
Week: 2
Home Goals: 0
Away Goals: 2

✓ Success: ID: 4
```

Then select option **5. Back to Main Menu**

### Phase 5: Verify Everything

Go to main menu → Select **5. Reports**

Check these reports:

**1. League Standings**
```
Expected output shows:
- Manchester United: 6 points
- Liverpool: 6 points
- Chelsea: 1 point
- Arsenal: 0 points
```

**2. Top Scorers**
```
Should show players with goals (0 goals initially since we didn't record goalscorers)
```

**3. Teams with Most Goals Scored**
```
Expected:
1. Manchester United: 3
2. Liverpool: 2
3. Chelsea: 1
4. Arsenal: 0
```

**4. Teams with Most Goals Conceded**
```
Expected:
1. Chelsea: 3
2. Arsenal: 2
3. Manchester United: 1
4. Liverpool: 1
```

## Summary: What You Created

### Stadiums
- Anfield (61,000 capacity)
- Old Trafford (75,000 capacity)
- Stamford Bridge (62,000 capacity)
- Emirates Stadium (60,000 capacity)

### Teams & Records (After 2 weeks)
1. **Liverpool** - 6 pts, 1W-0D-1L (2 GF, 2 GA, +0 GD)
2. **Manchester United** - 6 pts, 2W-0D-0L (3 GF, 1 GA, +2 GD)
3. **Chelsea** - 1 pt, 0W-1D-1L (1 GF, 3 GA, -2 GD)
4. **Arsenal** - 0 pts, 0W-0D-2L (0 GF, 5 GA, -5 GD)

### Matches
- Week 1: Liverpool 2-1 Manchester United
- Week 1: Chelsea 1-0 Arsenal
- Week 2: Manchester United 3-1 Chelsea
- Week 2: Arsenal 0-2 Liverpool

### Players Created
12 total players across 4 teams

## Next Steps

### Try These Operations

**1. View Team Details**
```
Team Management → View Team Details → Enter 2

Shows: Manchester United's stats and all their players
```

**2. Update a Match Score**
```
Match Management → Update Match → Enter 1

Change Liverpool 2-1 Man Utd to 3-1
Watch standings update!
```

**3. Add a New Team**
```
First create another stadium, then another team
```

**4. Delete and Re-create**
```
Delete a match and watch standings recalculate
Try deleting a team (should delete all its players too)
```

## Common Operations

### To see all data:
```
Stadium Management → View All Stadiums
Team Management → View All Teams
Player Management → View All Players
Match Management → View All Matches
```

### To modify data:
```
Update Team → Enter ID → Change name/code/stadium
Update Match → Enter ID → Change goals
```

### To generate reports:
```
Reports → Any of 4 options
```

## Troubleshooting

If you get an error:

1. **"Stadium with ID X not found"**
   - Stadium probably doesn't exist
   - Go View All Stadiums first to see what IDs exist
   - Use correct Stadium ID

2. **"Team name must be unique"**
   - You already created a team with that name
   - Use a different name

3. **"Team code must be unique"**
   - You already used that code (1-99)
   - Use a different code

4. **"Home team and away team cannot be the same"**
   - You entered the same team ID for both home and away
   - Use different team IDs

5. **"Jersey number already exists in this team"**
   - You're using the same jersey # in the same team
   - Use a different number (can be same across teams)

## Data Created Summary

After following this guide, you'll have:
- ✓ 4 stadiums
- ✓ 4 teams
- ✓ 12 players
- ✓ 4 matches
- ✓ Full standings with proper calculations
- ✓ Working reports

The application is now fully functional with realistic data!

## Advance: Add More Data

You can add more data at any time:
- More stadiums
- More teams (each needs a stadium)
- More players (each needs a team)
- More matches (update standings)

Just follow the same pattern: Parents before children, and always use existing IDs.
