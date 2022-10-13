using RPG.Combat.Domain;
using RPG.Combat.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RPG.Combat.Services.FightService
{
    public static class CombatManager
    {
        private static readonly int MinimalValueForDefensiveAttributes = 0;
        private static readonly int MinimalValueForOffensiveAttributes = 1;


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
        /// <para>
        /// Calculates the damage by the formula:
        /// - Attacker's weapon full damage + random value to the max of attacker's strength
        /// - minus a random value up to opponent's defense value
        /// </para>
        /// <para>This method ignores negative/zero values for weapon damage, attacker strength and opponentes defense.</para>
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="opponent"></param>
        /// <returns>Damage took</returns>
        public static int DoWeaponDamage(Character attacker, Character opponent)
        {
            var damage = CalculateDamage(attacker.Weapon.Damage, (CharacterAttribute.Strenght, attacker.Strength, attacker.AttributeModifiers), (opponent.Defense, opponent.AttributeModifiers));
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
            var damage = CalculateDamage(skill.Damage, (CharacterAttribute.Intelligence, attacker.Intelligence, attacker.AttributeModifiers), (opponent.Defense, opponent.AttributeModifiers));
            opponent.HitPoints -= damage;
            return damage;
        }

        private static int CalculateDamage(int attackValue, (CharacterAttribute attribute, int maxAttributeValue, List<AttributeModifier> modifiers) attackAttribute, (int maxDefenseValue, List<AttributeModifier> modifiers) defenseAttribute)
        {
            //calculates total attacker attribute values
            var positiveAttackModifiers = GetPositiveModifiersForAttribute(attackAttribute.attribute, attackAttribute.modifiers);
            var sumOfPositiveAttackModifiers = GetSumOfModifiers(positiveAttackModifiers);
            var sumOfNegativeAttackModifiers = GetSumOfModifiers(GetNegativeModifiersForAttribute(attackAttribute.attribute, attackAttribute.modifiers));
            int totalAttributeValue = (attackAttribute.maxAttributeValue + sumOfPositiveAttackModifiers) - sumOfNegativeAttackModifiers;
            var attributeValue = totalAttributeValue > 0 ? new Random().Next(MinimalValueForOffensiveAttributes, totalAttributeValue) : 0;
            int damage = GetPositiveValueOrZero(attackValue) + attributeValue;

            //calculates total opponent defense
            var positiveDefenseModifiers = GetPositiveModifiersForAttribute(CharacterAttribute.Defense, defenseAttribute.modifiers);
            var sumOfPositiveDefenseModifiers = GetSumOfModifiers(positiveDefenseModifiers);
            var sumOfNegativeDefenseModifiers = GetSumOfModifiers(GetNegativeModifiersForAttribute(CharacterAttribute.Defense, defenseAttribute.modifiers));
            var totalDefenseValue = (defenseAttribute.maxDefenseValue + sumOfPositiveDefenseModifiers) - sumOfNegativeDefenseModifiers;
            int defense = totalDefenseValue > 0 ? new Random().Next(MinimalValueForDefensiveAttributes, totalDefenseValue) : 0;
            
            damage -= defense;
            return damage < 0 ? 0 : damage;
        }
        public static List<AttributeModifier> GetPositiveModifiersForAttribute(CharacterAttribute attribute, List<AttributeModifier> modifiers = default)
        {
            if (modifiers is null)
                return new List<AttributeModifier>();

            return modifiers.Where(modifier => modifier.Attribute.Equals(attribute) && modifier.IsPositive).ToList();
        }

        public static List<AttributeModifier> GetNegativeModifiersForAttribute(CharacterAttribute attribute, List<AttributeModifier> modifiers = default)
        {
            if (modifiers is null)
                return new List<AttributeModifier>();

            return modifiers.Where(modifier => modifier.Attribute.Equals(attribute) && !modifier.IsPositive).ToList();
        }

        public static int GetSumOfModifiers(List<AttributeModifier> modifiers = default)
        {
            if (modifiers is null)
                return 0;
            return modifiers.Sum(modifier => modifier.Value);
        }

        /// <summary>
        /// Returns the provided value if it is positive, zero otherwise
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetPositiveValueOrZero(int value) => value > 0 ? value : 0;

    }
}
