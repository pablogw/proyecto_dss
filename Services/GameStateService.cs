using Fire_Emblem.Models;

namespace Fire_Emblem.Services
{
    public class GameStateService
    {
        public Team Player1Team { get; private set; }
        public Team Player2Team { get; private set; }
        public int CurrentRound { get; private set; } = 1;
        public bool IsPlayer1Turn { get; private set; } = true;

        public void InitializeGame(Team player1Team, Team player2Team)
        {
            Player1Team = player1Team;
            Player2Team = player2Team;
            CurrentRound = 1;
            IsPlayer1Turn = true;
        }

        public bool IsGameOver()
        {
            // Check if either team is null (which might indicate invalid teams)
            if (Player1Team == null || Player2Team == null)
            {
                return true; // Consider the game over if teams are not properly initialized
            }
    
            return Player1Team.IsDefeated || Player2Team.IsDefeated;
        }

        public void NextTurn()
        {
            IsPlayer1Turn = !IsPlayer1Turn;
            CurrentRound++;
        }

        public Team GetCurrentPlayerTeam()
        {
            return IsPlayer1Turn ? Player1Team : Player2Team;
        }

        public Team GetOpponentTeam()
        {
            return IsPlayer1Turn ? Player2Team : Player1Team;
        }
        
        public bool HasUnitWithDuplicateSkills(Team team)
        {
            return team.Units.Any(unit => HasDuplicateSkills(unit));
        }

        private bool HasDuplicateSkills(Unit unit)
        {
            return unit.Skills.GroupBy(skill => skill.Name, StringComparer.OrdinalIgnoreCase)
                .Any(group => group.Count() > 1);
        }

        public bool HasUnitWithDuplicateNames(Team team)
        {
            return team.Units.GroupBy(u => u.Name).Any(g => g.Count() > 1);
        }
        
        

       
    }
}