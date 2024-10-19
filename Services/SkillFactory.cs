using Fire_Emblem.Models;
using System;
using System.Collections.Generic;

namespace Fire_Emblem.Services
{
    public static class SkillFactory
    {
        public static Skill OtherSkill(string skillName)
        {
            return new BonusSkill(skillName, new Dictionary<string, int> { });
        }
        // Alteration of Stats Base
        public static Skill CreateHPPlus15()
        {
            return new BonusSkill("HP +15", new Dictionary<string, int> { { "MaxHP", 15 } });
        }

        // Bonus Skills
        public static Skill CreateFairFight()
        {
            return new HybridSkill("Fair Fight", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Atk", 6))
                .AddEffect(new StatModifier("Atk", 6, true, false)); // Apply to opponent
        }

        public static Skill CreateWillToWin()
        {
            return new HybridSkill("Will to win", (unit, oponent) => 
                {
                    bool shouldActivate = unit.CurrentHP < unit.MaxHP / 2;
                    return shouldActivate;
                })
                .AddEffect(new StatModifier("Atk", 8));
        }

        public static Skill CreateSingleMinded()
        {
            return new HybridSkill("Single-Minded", (unit, opponent) => opponent == unit.MostRecentOpponent)
                .AddEffect(new StatModifier("Atk", 8));
        }

        public static Skill CreateIgnis()
        {
            return new HybridSkill("Ignis")
                .AddEffect(new CustomEffect((unit, _) =>
                {
                    int bonusAtk = unit.Atk / 2;
                    unit.AddBonus("Atk", bonusAtk, true); // Apply to first attack only
                }));
        }

        public static Skill CreatePerceptive()
        {
            return new HybridSkill("Perceptive", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Spd", 12))
                .AddEffect(new CustomEffect((unit, _) =>
                {
                    int additionalSpd = unit.Spd / 4;
                    unit.AddBonus("Spd", additionalSpd);
                }));
        }

        public static Skill CreateTomePrecision()
        {
            return new HybridSkill("Tome Precision", (unit, _) => unit.Weapon == Weapon.Magic)
                .AddEffect(new StatModifier("Atk", 6))
                .AddEffect(new StatModifier("Spd", 6));
        }

        public static Skill CreateArmoredBlow()
        {
            return new HybridSkill("Armored Blow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Def", 8));
        }

        // Penalty Skills
        public static Skill CreateBlindingFlash()
        {
            return new HybridSkill("Blinding Flash", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Spd", -4, false, false)); // Apply to opponent
        }

        public static Skill CreateNotQuite()
        {
            return new HybridSkill("Not *Quite*", (_, opponent) => opponent.IsInitiatingCombat)
                .AddEffect(new StatModifier("Atk", -4, false, false)); // Apply to opponent
        }

        public static Skill CreateStunningSmile()
        {
            return new HybridSkill("Stunning Smile", (_, opponent) => opponent.Gender == "Male")
                .AddEffect(new StatModifier("Spd", -8, false, false)); // Apply to opponent
        }

        // ... Add other penalty skills similarly ...

        // Neutralization Skills
        public static Skill CreateBeorcBlessing()
        {
            return new HybridSkill("Beorc's Blessing")
                .AddEffect(new NeutralizeEffect(true, false, false)); // Neutralize opponent's bonuses
        }

        public static Skill CreateAgneaArrow()
        {
            return new HybridSkill("Agnea's Arrow")
                .AddEffect(new NeutralizeEffect(false, true, true)); // Neutralize unit's penalties
        }

        // Hybrid Skills
        public static Skill CreateSoulblade()
        {
            return new HybridSkill("Soulblade", (unit, _) => unit.Weapon == Weapon.Sword)
                .AddEffect(new CustomEffect((unit, opponent) =>
                {
                    int averageDefRes = (opponent.Def + opponent.Res) / 2;
                    int defModifier = averageDefRes - opponent.Def;
                    int resModifier = averageDefRes - opponent.Res;
                    opponent.AddBonus("Def", defModifier);
                    opponent.AddBonus("Res", resModifier);
                }));
        }

        public static Skill CreateSandstorm()
        {
            return new HybridSkill("Sandstorm")
                .AddEffect(new CustomEffect((unit, _) =>
                {
                    int newAtk = (int)(unit.Def * 1.5);
                    int atkModifier = newAtk - unit.Atk;
                    unit.AddBonus("Atk", atkModifier, false, true); // Apply to Follow-Up only
                }));
        }

        public static Skill CreateSwordAgility()
        {
            return new HybridSkill("Sword Agility", (unit, _) => unit.Weapon == Weapon.Sword)
                .AddEffect(new StatModifier("Spd", 12))
                .AddEffect(new StatModifier("Atk", -6));
        }

        public static Skill CreateCloseDefense()
        {
            return new HybridSkill("Close Def", (unit, opponent) => 
                !unit.IsInitiatingCombat && (opponent.Weapon == Weapon.Sword || opponent.Weapon == Weapon.Lance || opponent.Weapon == Weapon.Axe))
                .AddEffect(new StatModifier("Def", 8))
                .AddEffect(new StatModifier("Res", 8))
                .AddEffect(new NeutralizeEffect(true, false, false)); // Neutralize opponent's bonuses
        }

        public static Skill CreateLullAtkSpd()
        {
            return new HybridSkill("Lull Atk/Spd")
                .AddEffect(new StatModifier("Atk", -3, false, false)) // Penalty to opponent
                .AddEffect(new StatModifier("Spd", -3, false, false)) // Penalty to opponent
                .AddEffect(new CustomEffect((_, opponent) =>
                {
                    opponent.NeutralizeBonus("Atk");
                    opponent.NeutralizeBonus("Spd");
                }));
        }

        public static Skill CreateDragonskin()
        {
            return new HybridSkill("Dragonskin", (unit, opponent) => 
                !unit.IsInitiatingCombat || opponent.CurrentHP >= opponent.MaxHP * 0.75)
                .AddEffect(new StatModifier("Atk", 6))
                .AddEffect(new StatModifier("Spd", 6))
                .AddEffect(new StatModifier("Def", 6))
                .AddEffect(new StatModifier("Res", 6))
                .AddEffect(new NeutralizeEffect(true, false, false)); // Neutralize opponent's bonuses
        }

        public static Skill CreateAttackPlus6()
        {
            return new HybridSkill("Attack +6")
                .AddEffect(new StatModifier("Atk", 6));
        }

        public static Skill CreateSpeedPlus5()
        {
            return new HybridSkill("Speed +5")
                .AddEffect(new StatModifier("Spd", 5));
        }

        public static Skill CreateDefensePlus5()
        {
            return new HybridSkill("Defense +5")
                .AddEffect(new StatModifier("Def", 5));
        }

        public static Skill CreateWrath()
        {
            return new HybridSkill("Wrath")
                .AddEffect(new CustomEffect((unit, _) =>
                {
                    int lostHP = unit.MaxHP - unit.CurrentHP;
                    int bonus = Math.Min(lostHP, 30);
                    unit.AddBonus("Atk", bonus);
                    unit.AddBonus("Spd", bonus);
                }));
        }

        public static Skill CreateResolve()
        {
            return new HybridSkill("Resolve", (unit, _) => unit.CurrentHP <= unit.MaxHP * 0.75)
                .AddEffect(new StatModifier("Def", 7))
                .AddEffect(new StatModifier("Res", 7));
        }

        public static Skill CreateResistancePlus5()
        {
            return new HybridSkill("Resistance +5")
                .AddEffect(new StatModifier("Res", 5));
        }

        public static Skill CreateAtkDefPlus5()
        {
            return new HybridSkill("Atk/Def +5")
                .AddEffect(new StatModifier("Atk", 5))
                .AddEffect(new StatModifier("Def", 5));
        }

        public static Skill CreateAtkResPlus5()
        {
            return new HybridSkill("Atk/Res +5")
                .AddEffect(new StatModifier("Atk", 5))
                .AddEffect(new StatModifier("Res", 5));
        }

        public static Skill CreateSpdResPlus5()
        {
            return new HybridSkill("Spd/Res +5")
                .AddEffect(new StatModifier("Spd", 5))
                .AddEffect(new StatModifier("Res", 5));
        }

        public static Skill CreateDeadlyBlade()
        {
            return new HybridSkill("Deadly Blade", (unit, _) => unit.IsInitiatingCombat && unit.Weapon == Weapon.Sword)
                .AddEffect(new StatModifier("Atk", 8))
                .AddEffect(new StatModifier("Spd", 8));
        }

        public static Skill CreateDeathBlow()
        {
            return new HybridSkill("Death Blow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Atk", 8));
        }

        public static Skill CreateDartingBlow()
        {
            return new HybridSkill("Darting Blow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Spd", 8));
        }

        public static Skill CreateWardingBlow()
        {
            return new HybridSkill("Warding Blow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Res", 8));
        }

        public static Skill CreateSwiftSparrow()
        {
            return new HybridSkill("Swift Sparrow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Atk", 6))
                .AddEffect(new StatModifier("Spd", 6));
        }

        public static Skill CreateSturdyBlow()
        {
            return new HybridSkill("Sturdy Blow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Atk", 6))
                .AddEffect(new StatModifier("Def", 6));
        }

        public static Skill CreateMirrorStrike()
        {
            return new HybridSkill("Mirror Strike", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Atk", 6))
                .AddEffect(new StatModifier("Res", 6));
        }

        public static Skill CreateSteadyBlow()
        {
            return new HybridSkill("Steady Blow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Spd", 6))
                .AddEffect(new StatModifier("Def", 6));
        }

        public static Skill CreateSwiftStrike()
        {
            return new HybridSkill("Swift Strike", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Spd", 6))
                .AddEffect(new StatModifier("Res", 6));
        }

        public static Skill CreateBracingBlow()
        {
            return new HybridSkill("Bracing Blow", (unit, _) => unit.IsInitiatingCombat)
                .AddEffect(new StatModifier("Def", 6))
                .AddEffect(new StatModifier("Res", 6));
        }

        public static Skill CreateBrazenAtkSpd()
        {
            return new HybridSkill("Brazen Atk/Spd", (unit, _) => unit.CurrentHP <= unit.MaxHP * 0.8)
                .AddEffect(new StatModifier("Atk", 10))
                .AddEffect(new StatModifier("Spd", 10));
        }

        public static Skill CreateBrazenAtkDef()
        {
            return new HybridSkill("Brazen Atk/Def", (unit, _) => unit.CurrentHP <= unit.MaxHP * 0.8)
                .AddEffect(new StatModifier("Atk", 10))
                .AddEffect(new StatModifier("Def", 10));
        }

        public static Skill CreateBrazenAtkRes()
        {
            return new HybridSkill("Brazen Atk/Res", (unit, _) => unit.CurrentHP <= unit.MaxHP * 0.8)
                .AddEffect(new StatModifier("Atk", 10))
                .AddEffect(new StatModifier("Res", 10));
        }

        public static Skill CreateBrazenSpdDef()
        {
            return new HybridSkill("Brazen Spd/Def", (unit, _) => unit.CurrentHP <= unit.MaxHP * 0.8)
                .AddEffect(new StatModifier("Spd", 10))
                .AddEffect(new StatModifier("Def", 10));
        }

        public static Skill CreateBrazenSpdRes()
        {
            return new HybridSkill("Brazen Spd/Res", (unit, _) => unit.CurrentHP <= unit.MaxHP * 0.8)
                .AddEffect(new StatModifier("Spd", 10))
                .AddEffect(new StatModifier("Res", 10));
        }

        public static Skill CreateBrazenDefRes()
        {
            return new HybridSkill("Brazen Def/Res", (unit, _) => unit.CurrentHP <= unit.MaxHP * 0.8)
                .AddEffect(new StatModifier("Def", 10))
                .AddEffect(new StatModifier("Res", 10));
        }

        public static Skill CreateFireBoost()
        {
            return new HybridSkill("Fire Boost", (unit, opponent) => unit.CurrentHP >= opponent.CurrentHP + 3)
                .AddEffect(new StatModifier("Atk", 6));
        }

        public static Skill CreateWindBoost()
        {
            return new HybridSkill("Wind Boost", (unit, opponent) => unit.CurrentHP >= opponent.CurrentHP + 3)
                .AddEffect(new StatModifier("Spd", 6));
        }

        public static Skill CreateEarthBoost()
        {
            return new HybridSkill("Earth Boost", (unit, opponent) => unit.CurrentHP >= opponent.CurrentHP + 3)
                .AddEffect(new StatModifier("Def", 6));
        }

        public static Skill CreateWaterBoost()
        {
            return new HybridSkill("Water Boost", (unit, opponent) => unit.CurrentHP >= opponent.CurrentHP + 3)
                .AddEffect(new StatModifier("Res", 6));
        }

        public static Skill CreateChaosStyle()
        {
            return new HybridSkill("Chaos Style", (unit, opponent) => 
                (unit.IsInitiatingCombat && unit.Weapon.IsPhysical() && opponent.Weapon == Weapon.Magic) ||
                (unit.IsInitiatingCombat && unit.Weapon == Weapon.Magic && opponent.Weapon.IsPhysical()))
                .AddEffect(new StatModifier("Spd", 3));
        }
    }
}