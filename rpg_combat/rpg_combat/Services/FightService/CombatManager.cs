using rpg_combat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_combat.Services.FightService
{
    public static class CombatManager
    {
        public enum AttackOptions
        {
            Weapon,
            Skill,
            DoNothing
        }


        /// <summary>
        /// Has a 1/30 chance of doing nothing. 
        /// Otherwise a 50/50 chance of attacking with weapon or skill.
        /// </summary>
        /// <returns>attack option</returns>
        public static AttackOptions DefineAttackOption()
        {
            var random = new Random();
            var shouldDoNothing = random.Next(30) == 15;
            if (shouldDoNothing)
                return AttackOptions.DoNothing;

            return random.Next(2) == 0 ? AttackOptions.Weapon : AttackOptions.Skill;
        }

        /// <summary>
        /// Calculates the damage by the formula:
        /// - Attacker's weapon full damage + random value to the max of attacker's strength
        /// - minus the opponent's defense value
        /// 
        /// This method ignores negative/zero values for weapon damage, attacker strength and opponentes defense.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="opponent"></param>
        /// <returns>Damage took</returns>
        public static int DoWeaponDamage(Character attacker, Character opponent)
        {
            var strength = attacker.Strength > 0 ? new Random().Next(1, attacker.Strength) : 0;
            int damage = GetPositiveValueOrZero(attacker.Weapon.Damage) + strength;
            damage -= opponent.Defense > 0 ? new Random().Next(1, opponent.Defense) : 0;
            if (damage < 0)
            {
                damage = 0;
            }
            opponent.HitPoints -= damage;
            return damage;
        }

        public static int DoSkillDamage(Character attacker, Character opponent, Skill skill)
        {
            var intelligence = attacker.Intelligence > 0 ? new Random().Next(1, attacker.Intelligence) : 0;
            int damage = GetPositiveValueOrZero(skill.Damage) + intelligence;
            damage -= opponent.Defense > 0 ? new Random().Next(1, opponent.Defense) : 0;
            if (damage < 0)
            {
                damage = 0;
            }
            opponent.HitPoints -= damage;
            return damage;
        }

        public static int GetPositiveValueOrZero(int value) => value > 0 ? value : 0;

    }
}
