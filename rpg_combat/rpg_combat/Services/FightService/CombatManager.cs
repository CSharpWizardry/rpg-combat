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
        /// - minus a random value up to opponent's defense value
        /// 
        /// This method ignores negative/zero values for weapon damage, attacker strength and opponentes defense.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="opponent"></param>
        /// <returns>Damage took</returns>
        public static int DoWeaponDamage(Character attacker, Character opponent)
        {
            var damage = CalculateDamage(attacker.Weapon.Damage, attacker.Strength, opponent.Defense);
            opponent.HitPoints -= damage;
            return damage;
        }

        /// <summary>
        /// Calculates the damage by the formula:
        /// - Attacker's skill full damage + random value to the max of attacker's Intelligence
        /// - minus a random value up to opponent's defense value
        /// 
        /// This method ignores negative/zero values for skill damage, attacker intelligence and opponentes defense.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="opponent"></param>
        /// <param name="skill"></param>
        /// <returns>Damage took</returns>
        public static int DoSkillDamage(Character attacker, Character opponent, Skill skill)
        {
            var damage = CalculateDamage(skill.Damage, attacker.Intelligence, opponent.Defense);
            opponent.HitPoints -= damage;
            return damage;
        }

        private static int CalculateDamage(int attackValue, int maxAttributeValue, int maxDefenseValue)
        {
            var attributeValue = maxAttributeValue > 0 ? new Random().Next(1, maxAttributeValue) : 0;
            int damage = GetPositiveValueOrZero(attackValue) + attributeValue;
            damage -= maxDefenseValue > 0 ? new Random().Next(1, maxDefenseValue) : 0;
            return damage < 0 ? 0 : damage;
        }

        /// <summary>
        /// Returns the provided value if it is positive, zero otherwise
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetPositiveValueOrZero(int value) => value > 0 ? value : 0;

    }
}
