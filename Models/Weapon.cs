namespace Fire_Emblem.Models
{
    public enum Weapon
    {
        Sword,
        Lance,
        Axe,
        Bow,
        Magic
    }

    public static class WeaponExtensions
    {
        public static bool HasAdvantageOver(this Weapon attacker, Weapon defender)
        {
            return (attacker, defender) switch
            {
                (Weapon.Sword, Weapon.Axe) => true,
                (Weapon.Axe, Weapon.Lance) => true,
                (Weapon.Lance, Weapon.Sword) => true,
                _ => false
            };
        }

        public static bool IsPhysical(this Weapon weapon)
        {
            return weapon != Weapon.Magic;
        }
    }
}