using Fire_Emblem.Models;
using Fire_Emblem_View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fire_Emblem.Services
{
    public class CombatService
    {
        private readonly View _view;

        public CombatService(View view)
        {
            _view = view;
        }

        public void ExecuteCombat(Unit attacker, Unit defender)
        {
            attacker.SetAsInitiator();
            ApplyPreCombatSkills(attacker, defender);
            DisplayActiveSkillEffects(attacker);
            DisplayActiveSkillEffects(defender);

            PerformAttack(attacker, defender, isFirstAttack: true);
            if (defender.IsAlive)
            {
                PerformAttack(defender, attacker, isFirstAttack: true);
            }
            PerformFollowUpAttacks(attacker, defender);

            ApplyPostCombatSkills(attacker, defender);
            ResetCombatEffects(attacker, defender);
        }

        private void ApplyPreCombatSkills(Unit attacker, Unit defender)
        {
            ApplySkills(attacker, defender);
            ApplySkills(defender, attacker);
        }

        private void ApplySkills(Unit unit, Unit opponent)
        {
            foreach (var skill in unit.Skills)
            {
                if (skill.ShouldActivate(unit, opponent))
                {
                    skill.ApplyEffect(unit, opponent);
                }
            }
        }

        private void DisplayActiveSkillEffects(Unit unit)
        {
            DisplayStatModifiers(unit, "bonus", unit.GetAllBonuses());
            DisplayStatModifiers(unit, "penalty", unit.GetAllPenalties());
            DisplaySpecificAttackModifiers(unit);
            DisplayNeutralizations(unit);
        }

        private void DisplayStatModifiers(Unit unit, string type, Dictionary<string, int> modifiers)
        {
            foreach (var kvp in modifiers)
            {
                _view.WriteLine($"{unit.Name} obtiene {kvp.Key}{(type == "bonus" ? "+" : "-")}{Math.Abs(kvp.Value)}");
            }
        }


        private void DisplaySpecificAttackModifiers(Unit unit)
        {
            DisplaySpecificModifiers(unit, "bonus", unit.GetSpecificAttackBonuses());
            DisplaySpecificModifiers(unit, "penalty", unit.GetSpecificAttackPenalties());
        }

        private void DisplaySpecificModifiers(Unit unit, string type, Dictionary<string, Dictionary<string, int>> modifiers)
        {
            foreach (var attackType in modifiers.Keys)
            {
                foreach (var kvp in modifiers[attackType])
                {
                    string message = $"{unit.Name} obtiene {kvp.Key}{(type == "bonus" ? "+" : "-")}{Math.Abs(kvp.Value)} en su {attackType}";
                    _view.WriteLine(message);
                }
            }
        }

        
        private void DisplayNeutralizations(Unit unit)
        {
            foreach (var stat in unit.GetNeutralizedBonuses())
            {
                _view.WriteLine($"Los bonus de {stat} de {unit.Name} fueron neutralizados");
            }
            foreach (var stat in unit.GetNeutralizedPenalties())
            {
                _view.WriteLine($"Los penalty de {stat} de {unit.Name} fueron neutralizados");
            }
        }

        private void PerformAttack(Unit attacker, Unit defender, bool isFirstAttack)
        {
            int damage = CalculateDamage(attacker, defender, isFirstAttack);
            defender.TakeDamage(damage);
            _view.WriteLine($"{attacker.Name} ataca a {defender.Name} con {damage} de daño");
        }
        
        private void PerformFollowUpAttacks(Unit attacker, Unit defender)
        {
            if (!BothUnitsAreAlive(attacker, defender))
            {
                return;
            }

            if (TryPerformFollowUp(attacker, defender) || TryPerformFollowUp(defender, attacker))
            {
                return;
            }

            _view.WriteLine("Ninguna unidad puede hacer un follow up");
        }

        private bool BothUnitsAreAlive(Unit unit1, Unit unit2)
        {
            return unit1.IsAlive && unit2.IsAlive;
        }

        private bool TryPerformFollowUp(Unit attacker, Unit defender)
        {
            if (CanPerformFollowUp(attacker, defender) && BothUnitsAreAlive(attacker, defender))
            {
                PerformAttack(attacker, defender, isFirstAttack: false);
                return true;
            }
            return false;
        }

        private int CalculateDamage(Unit attacker, Unit defender, bool isFirstAttack)
        {
            double weaponTriangleModifier = GetWeaponTriangleModifier(attacker.Weapon, defender.Weapon);
            int attackPower = (int)(attacker.GetEffectiveStat("Atk", isFirstAttack, !isFirstAttack) * weaponTriangleModifier);
            string defenseType = attacker.Weapon.IsPhysical() ? "Def" : "Res";
            int defensePower = defender.GetEffectiveStat(defenseType, isFirstAttack, !isFirstAttack);
            return Math.Max(0, attackPower - defensePower);
        }

        private double GetWeaponTriangleModifier(Weapon attackerWeapon, Weapon defenderWeapon)
        {
            if (attackerWeapon.HasAdvantageOver(defenderWeapon)) return 1.2;
            if (defenderWeapon.HasAdvantageOver(attackerWeapon)) return 0.8;
            return 1.0;
        }

        private bool CanPerformFollowUp(Unit unit, Unit opponent)
        {
            return unit.GetEffectiveStat("Spd") >= opponent.GetEffectiveStat("Spd") + 5;
        }

        private void ApplyPostCombatSkills(Unit attacker, Unit defender)
        {
            // Implement post-combat skill effects here if needed
        }

        private void ResetCombatEffects(Unit attacker, Unit defender)
        {
            attacker.ResetCombatEffects();
            defender.ResetCombatEffects();
        }

        public Unit SelectUnit(Team team, string action, Team player1Team)
        {
            _view.WriteLine($"Player {(team == player1Team ? "1" : "2")} selecciona una opción");
            List<Unit> aliveUnits = team.GetAliveUnits().ToList();
            for (int i = 0; i < aliveUnits.Count; i++)
            {
                _view.WriteLine($"{i}: {aliveUnits[i].Name}");
            }
            int selectedUnitIndex = int.Parse(_view.ReadLine());
            return aliveUnits[selectedUnitIndex];
        }

        public void DisplayWeaponAdvantage(Unit attacker, Unit defender)
        {
            if (attacker.Weapon.HasAdvantageOver(defender.Weapon))
            {
                _view.WriteLine($"{attacker.Name} ({attacker.Weapon}) tiene ventaja con respecto a {defender.Name} ({defender.Weapon})");
            }
            else if (defender.Weapon.HasAdvantageOver(attacker.Weapon))
            {
                _view.WriteLine($"{defender.Name} ({defender.Weapon}) tiene ventaja con respecto a {attacker.Name} ({attacker.Weapon})");
            }
            else
            {
                _view.WriteLine("Ninguna unidad tiene ventaja con respecto a la otra");
            }
        }

        public void DisplayCombatResult(Unit attacker, Unit defender)
        {
            _view.WriteLine($"{attacker.Name} ({attacker.CurrentHP}) : {defender.Name} ({defender.CurrentHP})");
        }
    }
}