# Sample Data Setup Guide

Use this guide to quickly populate your database with sample data for testing.

## Create Sample Stadiums

1. Go to "Stadium Management" → "Create Stadium"
2. Create these stadiums:

| Stadium Name      | Capacity |
|-------------------|----------|
| Old Trafford      | 75000    |
| Stamford Bridge   | 62000    |
| Anfield           | 61000    |
| Emirates Stadium  | 60000    |

## Create Sample Teams

1. Go to "Team Management" → "Create Team"
2. Create these teams:

| Team Name            | Code | Stadium ID |
|----------------------|------|-----------|
| Manchester United    | 1    | 1         |
| Chelsea              | 2    | 2         |
| Liverpool            | 3    | 3         |
| Arsenal              | 4    | 4         |

## Create Sample Players

### Manchester United (Team ID: 1)
1. Jersey 7, David Beckham
2. Jersey 9, Wayne Rooney
3. Jersey 1, Edwin van der Sar

### Chelsea (Team ID: 2)
1. Jersey 11, Frank Lampard
2. Jersey 15, Didier Drogba
3. Jersey 1, Petr Cech

### Liverpool (Team ID: 3)
1. Jersey 7, Kenny Dalglish
2. Jersey 9, Robbie Fowler
3. Jersey 1, Ray Clemence

### Arsenal (Team ID: 4)
1. Jersey 14, Thierry Henry
2. Jersey 4, Patrick Vieira
3. Jersey 1, Edwin Kraft

## Create Sample Matches

1. Go to "Match Management" → "Create Match"
2. Create these matches:

### Week 1
- Home: Manchester United (1), Away: Chelsea (2)
  - Goals: 2-1
- Home: Liverpool (3), Away: Arsenal (4)
  - Goals: 3-0

### Week 2
- Home: Chelsea (2), Away: Manchester United (1)
  - Goals: 1-2
- Home: Arsenal (4), Away: Liverpool (3)
  - Goals: 1-2

## Expected Results After Sample Data

### League Table (after Week 2)
1. Manchester United: 6 points, +2 GD
2. Liverpool: 6 points, +2 GD (behind on goals)
3. Chelsea: 0 points
4. Arsenal: 0 points

### Top Scorers
- Thierry Henry (Arsenal): 0 goals
- All other players: 0 goals

(Note: Adjust based on actual match results entered)

## Testing Checklist

After creating sample data, verify:

✓ All teams appear in standings
✓ Goal differences calculate correctly
✓ Points are assigned properly (3 for win, 1 for draw)
✓ Player information displays correctly
✓ Reports show all expected data
✓ League standings sort correctly
✓ Duplicate team names are rejected
✓ Duplicate team codes are rejected
✓ Duplicate player jersey numbers in same team are rejected

## Advanced Testing

### Test Win/Loss/Draw Scenarios
- Create matches with equal scores (draws)
- Verify 1 point awarded to each team
- Verify points calculation in standings

### Test Data Integrity
- Try to create duplicate team names (should fail)
- Try to create duplicate team codes (should fail)
- Try to create match with same home/away team (should fail)
- Try to delete team with players (should cascade delete)

### Test Updates
- Update match score and verify statistics recalculate
- Update player information
- Delete match and verify statistics update

## Tips for Manual Testing

1. **Keep track of IDs**: Write down the IDs created so you can reference them
2. **Create systematically**: Stadiums → Teams → Players → Matches
3. **Verify each step**: Use "View All" options to confirm data was created
4. **Test edge cases**: Try operations that should fail to verify validation
5. **Check reports**: After each match, view standings to verify calculations

## Performance Considerations

- Application loads all data into memory
- For production: consider pagination and filtering
- Current design suitable for small to medium leagues
- Statistics recalculation is O(n) where n is number of matches
