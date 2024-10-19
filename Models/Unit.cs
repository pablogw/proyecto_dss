using Fire_Emblem_View;
using System;
using System.Collections.Generic;

namespace Fire_Emblem.Models
{
    public class Unit
    {
        public string Name { get; set; }
        public Weapon Weapon { get; set; }
        public string Gender { get; set; }
        public int MaxHP { get; set; }
        public int CurrentHP { get; set; }
        public int Atk { get; set; }
        public int Spd { get; set; }
        public int Def { get; set; }
        public int Res { get; set; }
        public List<Skill> Skills { get; set; } = new List<Skill>();

        public bool IsAlive => CurrentHP > 0;
        public bool IsInitiatingCombat { get; set; }
        public Unit MostRecentOpponent { get; set; }

        private Dictionary<string, StatModifiers> _statModifiers = new Dictionary<string, StatModifiers>();
        private HashSet<string> NeutralizedBonuses = new HashSet<string>();
        private HashSet<string> NeutralizedPenalties = new HashSet<string>();

        public Unit(string name, Weapon weapon, string gender, int maxHP, int atk, int spd, int def, int res)
        {
            Name = name;
            Weapon = weapon;
            Gender = gender;
            MaxHP = maxHP;
            CurrentHP = maxHP;
            Atk = atk;
            Spd = spd;
            Def = def;
            Res = res;
        }

        public void TakeDamage(int damage)
        {
            CurrentHP = Math.Max(0, CurrentHP - damage);
        }

        public void AddSkill(Skill skill)
        {
            Skills.Add(skill);
        }

        public void AddBonus(string stat, int value, bool applyToFirstAttack = false, bool applyToFollowUp = false)
        {
            if (!_statModifiers.ContainsKey(stat))
                _statModifiers[stat] = new StatModifiers();
    
            if (applyToFirstAttack)
                _statModifiers[stat].FirstAttackBonus += value;
            else if (applyToFollowUp)
                _statModifiers[stat].FollowUpBonus += value;
            else
                _statModifiers[stat].GeneralBonus += value;
    
            Console.WriteLine($"DEBUG: Added bonus to {Name}: {stat}+{value}");
        }

        public void AddPenalty(string stat, int value, bool applyToFirstAttack = false, bool applyToFollowUp = false)
        {
            if (!_statModifiers.ContainsKey(stat))
                _statModifiers[stat] = new StatModifiers();
            
            if (applyToFirstAttack)
                _statModifiers[stat].FirstAttackPenalty += value;
            else if (applyToFollowUp)
                _statModifiers[stat].FollowUpPenalty += value;
            else
                _statModifiers[stat].GeneralPenalty += value;
        }

        public void NeutralizeAllBonuses()
        {
            NeutralizedBonuses.UnionWith(_statModifiers.Keys);
        }

        public void NeutralizeAllPenalties()
        {
            NeutralizedPenalties.UnionWith(_statModifiers.Keys);
        }

        public void NeutralizeBonus(string stat)
        {
            NeutralizedBonuses.Add(stat);
        }

        public void NeutralizePenalty(string stat)
        {
            NeutralizedPenalties.Add(stat);
        }

        public int GetEffectiveStat(string stat, bool isFirstAttack = false, bool isFollowUp = false)
        {
            int baseStat = stat switch
            {
                "Atk" => Atk,
                "Spd" => Spd,
                "Def" => Def,
                "Res" => Res,
                "MaxHP" => MaxHP,
                _ => throw new ArgumentException("Invalid stat name", nameof(stat))
            };

            if (!_statModifiers.ContainsKey(stat))
                return baseStat;

            var modifiers = _statModifiers[stat];
            int bonus = NeutralizedBonuses.Contains(stat) ? 0 : modifiers.GetTotalBonus(isFirstAttack, isFollowUp);
            int penalty = NeutralizedPenalties.Contains(stat) ? 0 : modifiers.GetTotalPenalty(isFirstAttack, isFollowUp);

            return baseStat + bonus - penalty;
        }

        public Dictionary<string, int> GetAllBonuses()
        {
            var allBonuses = new Dictionary<string, int>();
            foreach (var kvp in _statModifiers)
            {
                int totalBonus = kvp.Value.GetTotalBonus(false, false);
                if (totalBonus != 0)
                {
                    allBonuses[kvp.Key] = totalBonus;
                }
            }
            return allBonuses;
        }

        public Dictionary<string, int> GetAllPenalties()
        {
            var allPenalties = new Dictionary<string, int>();
            foreach (var kvp in _statModifiers)
            {
                int totalPenalty = kvp.Value.GetTotalPenalty(false, false);
                if (totalPenalty != 0)
                {
                    allPenalties[kvp.Key] = totalPenalty;
                }
            }
            return allPenalties;
        }

        public IEnumerable<string> GetNeutralizedBonuses()
        {
            return NeutralizedBonuses;
        }

        public IEnumerable<string> GetNeutralizedPenalties()
        {
            return NeutralizedPenalties;
        }
        public void ResetCombatEffects()
        {
            Console.WriteLine($"DEBUG: Resetting combat effects for {Name}");
            _statModifiers.Clear();
            NeutralizedBonuses.Clear();
            NeutralizedPenalties.Clear();
            IsInitiatingCombat = false;
            Console.WriteLine($"DEBUG: After reset, {Name}'s stats:");
            Console.WriteLine($"DEBUG: Atk: {GetEffectiveStat("Atk")}");
            Console.WriteLine($"DEBUG: Spd: {GetEffectiveStat("Spd")}");
            Console.WriteLine($"DEBUG: Def: {GetEffectiveStat("Def")}");
            Console.WriteLine($"DEBUG: Res: {GetEffectiveStat("Res")}");
        }

        public void SetAsInitiator()
        {
            IsInitiatingCombat = true;
        }

        public void SetMostRecentOpponent(Unit opponent)
        {
            MostRecentOpponent = opponent;
        }
        
        public Dictionary<string, Dictionary<string, int>> GetSpecificAttackBonuses()
        {
            return GetSpecificModifiers(true);
        }

        public Dictionary<string, Dictionary<string, int>> GetSpecificAttackPenalties()
        {
            return GetSpecificModifiers(false);
        }

        private Dictionary<string, Dictionary<string, int>> GetSpecificModifiers(bool isBonus)
        {
            var result = new Dictionary<string, Dictionary<string, int>>
            {
                ["primer ataque"] = new Dictionary<string, int>(),
                ["Follow-Up"] = new Dictionary<string, int>()
            };

            foreach (var kvp in _statModifiers)
            {
                int generalModifier = isBonus ? kvp.Value.GeneralBonus : kvp.Value.GeneralPenalty;
                int firstAttackModifier = isBonus ? kvp.Value.FirstAttackBonus : kvp.Value.FirstAttackPenalty;
                int followUpModifier = isBonus ? kvp.Value.FollowUpBonus : kvp.Value.FollowUpPenalty;

                if (firstAttackModifier != 0 && firstAttackModifier != generalModifier)
                {
                    result["primer ataque"][kvp.Key] = firstAttackModifier;
                }
                if (followUpModifier != 0 && followUpModifier != generalModifier)
                {
                    result["Follow-Up"][kvp.Key] = followUpModifier;
                }
            }

            return result;
        }

        private class StatModifiers
        {
            public int GeneralBonus { get; set; }
            public int FirstAttackBonus { get; set; }
            public int FollowUpBonus { get; set; }
            public int GeneralPenalty { get; set; }
            public int FirstAttackPenalty { get; set; }
            public int FollowUpPenalty { get; set; }

            public int GetTotalBonus(bool isFirstAttack, bool isFollowUp)
            {
                return GeneralBonus + (isFirstAttack ? FirstAttackBonus : 0) + (isFollowUp ? FollowUpBonus : 0);
            }

            public int GetTotalPenalty(bool isFirstAttack, bool isFollowUp)
            {
                return GeneralPenalty + (isFirstAttack ? FirstAttackPenalty : 0) + (isFollowUp ? FollowUpPenalty : 0);
            }
        }
    }
}