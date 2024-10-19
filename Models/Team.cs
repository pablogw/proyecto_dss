namespace Fire_Emblem.Models
{
    public class Team
    {
        public List<Unit> Units { get; private set; } = new List<Unit>();

        public bool IsDefeated => Units.All(unit => !unit.IsAlive);

        public void AddUnit(Unit unit)
        {
            if (Units.Count < 4)
            {
                Units.Add(unit);
            }
        }

        public Unit GetAliveUnit(int index)
        {
            return Units.Where(u => u.IsAlive).ElementAtOrDefault(index);
        }

        public IEnumerable<Unit> GetAliveUnits()
        {
            return Units.Where(u => u.IsAlive);
        }
    }
}