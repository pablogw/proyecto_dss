using Fire_Emblem.Models;

namespace Fire_Emblem.Utils
{
    public static class EnumExtensions
    {
        public static bool IsPhysicalWeapon(this Weapon weapon)
        {
            return weapon == Weapon.Sword || weapon == Weapon.Lance || weapon == Weapon.Axe || weapon == Weapon.Bow;
        }

        public static string ToFriendlyString(this Weapon weapon)
        {
            return weapon switch
            {
                Weapon.Sword => "Sword",
                Weapon.Lance => "Lance",
                Weapon.Axe => "Axe",
                Weapon.Bow => "Bow",
                Weapon.Magic => "Magic",
                _ => throw new ArgumentOutOfRangeException(nameof(weapon), weapon, null)
            };
        }
    }
}