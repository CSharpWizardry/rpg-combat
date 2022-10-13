using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPG.Combat.Models;
using RPG.Combat.Services.FightService;
using System.Collections.Generic;

namespace RPG.Combat.Test.Services
{
    [TestClass]
    public class CombatManagerTest
    {
        private Character attacker;
        private Character opponent;

        #region Damage tests
        #region Test Weapon
        [TestMethod]
        public void DoWeaponDamageShouldReturnZeroWhenDefenseIsGreaterThanAttackValue()
        {
            //Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Strength = 0;
            attacker.Weapon.Damage = 0;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 100;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.AreEqual(0, damage);
        }

        [TestMethod]
        public void DoWeaponDamageShouldReturnDamageValueWhenAttackValueIsGreaterThanDefense()
        {
            //Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = 20;
            attacker.Strength = 0;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 0;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.AreEqual(20, damage);
        }

        [TestMethod]
        public void DoWeaponDamageShouldReduceOpponentsLifeWhenAttackValueIsGreaterThanDefense()
        {
            //Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = 20;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 0;
            opponent.HitPoints = 100;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.IsTrue(opponent.HitPoints < 100);
            Assert.IsTrue(opponent.HitPoints == (100 - damage));
        }
        
        [TestMethod]
        public void DoWeaponDamageShouldIgnoreNegativeAttackerStrenghtAttribute()
        {
            //Arrange
            int maxDamage = 20;
            int opponentsDefense = 0;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = maxDamage;
            attacker.Strength = -10;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.IsTrue(opponent.HitPoints < maxHitPoints && opponent.HitPoints == 80);
            Assert.AreEqual(maxDamage, damage);
        }

        [TestMethod]
        public void DoWeaponDamageShouldIgnoreNegativeWeaponDamageValue()
        {
            //Arrange
            int maxDamage = 20;
            int opponentsDefense = 0;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = -20;
            attacker.Strength = maxDamage;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.IsTrue(opponent.HitPoints < maxHitPoints);
        }

        [TestMethod]
        public void DoWeaponDamageShouldIgnoreNegativeOpponentDefense()
        {
            //Arrange
            int maxDamage = 20;
            int opponentsDefense = -50;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = maxDamage;
            attacker.Strength = 0;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.AreEqual(80, opponent.HitPoints);
            Assert.AreEqual(maxDamage, damage);
        }
        #endregion

        #region Test Skill

        [TestMethod]
        public void DoSkillDamageShouldReturnZeroWhenDefenseIsGreaterThanAttackValue()
        {
            //ATTENTION! since defense is a random value between 1 and max defense we can't
            //really guarantee that it will be greater than the attack value of 1 so flake tests can exist
            //Arrange
            int opponentsDefense = 100;
            int skillMaxDamage = 1;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Intelligence = 0;
            var skill = attacker.CharacterSkills.Find(s => s.SkillId == 1).Skill;
            skill.Damage = skillMaxDamage;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoSkillDamage(attacker, opponent, skill);
            //Assert
            Assert.AreEqual(maxHitPoints, opponent.HitPoints);
            Assert.AreEqual(0, damage);
        }

        [TestMethod]
        public void DoSkillDamageShouldReturnDamageValueWhenAttackValueIsGreaterThanDefense()
        {
            int opponentsDefense = 0;
            int skillMaxDamage = 20;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Intelligence = 10;
            var skill = attacker.CharacterSkills.Find(s => s.SkillId == 1).Skill;
            skill.Damage = skillMaxDamage;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoSkillDamage(attacker, opponent, skill);
            //Assert
            Assert.IsTrue(damage >= 20 && damage <= (skillMaxDamage + attacker.Intelligence));
            Assert.IsTrue(opponent.HitPoints < maxHitPoints);
        }

        [TestMethod]
        public void DoSkillDamageShouldIgnoreNegativeAttackerStrenghtAttribute()
        {
            int opponentsDefense = 1;
            int skillMaxDamage = 20;
            int inteliggence = -14;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Intelligence = inteliggence;
            var skill = attacker.CharacterSkills.Find(s => s.SkillId == 1).Skill;
            skill.Damage = skillMaxDamage;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoSkillDamage(attacker, opponent, skill);
            //Assert
            Assert.IsTrue(damage >= 19);
            Assert.IsTrue(opponent.HitPoints < maxHitPoints);
        }

        [TestMethod]
        public void DoSkillDamageShouldIgnoreNegativeSkillDamageValue()
        {
            //This method can be a flake since both intelligence and defense can have 1 as value and 
            //the total damage would be zero
            int opponentsDefense = 1;
            int skillMaxDamage = -20;
            int inteliggence = 20;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Intelligence = inteliggence;
            var skill = attacker.CharacterSkills.Find(s => s.SkillId == 1).Skill;
            skill.Damage = skillMaxDamage;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoSkillDamage(attacker, opponent, skill);
            //Assert
            Assert.IsTrue(damage > 0);
            Assert.IsTrue(opponent.HitPoints < maxHitPoints);
        }

        [TestMethod]
        public void DoSkillDamageShouldIgnoreNegativeOpponentDefense()
        {
            //Arrange
            int skillMaxDamage = 20;
            int opponentsDefense = -50;
            int maxHitPoints = 100;
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            var skill = attacker.CharacterSkills.Find(s => s.SkillId == 1).Skill;
            skill.Damage = skillMaxDamage;
            attacker.Intelligence = 0;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = opponentsDefense;
            opponent.HitPoints = maxHitPoints;
            //Act
            var damage = CombatManager.DoSkillDamage(attacker, opponent, skill);
            //Assert
            Assert.AreEqual(80, opponent.HitPoints);
            Assert.AreEqual(skillMaxDamage, damage);
        }
        #endregion

        #region Modifiers
        [TestMethod]
        public void DoWeaponDamageWithAttackStrenghtModifiersShouldDoMoreDamage()
        {
            int weaponDamage = 20;
            // Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = weaponDamage;
            attacker.Strength = 0;
            attacker.AttributeModifiers.Add(new AttributeModifier(CharacterAttribute.Strenght, true, 20));
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 0;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.IsTrue(damage > weaponDamage);
        }        
        [TestMethod]
        public void DoWeaponDamageShouldConsiderNegativeAttackStrenghtModifier()
        {
            int weaponDamage = 20;
            // Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = weaponDamage;
            attacker.Strength = 20;
            attacker.AttributeModifiers.Add(new AttributeModifier(CharacterAttribute.Strenght, false, 20));
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 0;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.AreEqual(weaponDamage, damage);
        }
        [TestMethod]
        public void DoWeaponDamageShouldIgnoreOtherAttackPositiveModifiers()
        {
            int weaponDamage = 20;
            // Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = weaponDamage;
            attacker.Strength = 0;
            attacker.AttributeModifiers.Add(new AttributeModifier(CharacterAttribute.Intelligence, true, 20));
            attacker.AttributeModifiers.Add(new AttributeModifier(CharacterAttribute.Defense, true, 20));
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 0;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.AreEqual(weaponDamage, damage);
        }

        [TestMethod]
        public void DamageCalculationShouldConsiderPositiveDefenseModifier()
        {
            int weaponDamage = 20;
            // Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = weaponDamage;
            attacker.Strength = 0;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 0;
            opponent.AttributeModifiers.Add(new AttributeModifier(CharacterAttribute.Defense, true, 20));
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.AreNotEqual(weaponDamage, damage);
        }
        [TestMethod]
        public void DamageCalculationShouldConsiderNegativeDefenseModifier()
        {
            int weaponDamage = 20;
            // Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = weaponDamage;
            attacker.Strength = 0;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 20;
            opponent.AttributeModifiers.Add(new AttributeModifier(CharacterAttribute.Defense, false, 20));
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.AreEqual(weaponDamage, damage);
        }
        #endregion

        #region Test Utility Methods
        [TestMethod]
        public void GetPositiveValueOrZeroShouldReturnZeroWhenValueIsNegativeOrZero()
        {
            //Assert
            int testValue = -3;
            int testValue2 = 0;
            int testValue3 = -1;
            //Act
            var result = CombatManager.GetPositiveValueOrZero(testValue);
            var result2 = CombatManager.GetPositiveValueOrZero(testValue2);
            var result3 = CombatManager.GetPositiveValueOrZero(testValue3);
            //Assert
            Assert.AreEqual(0, result);
            Assert.AreEqual(0, result2);
            Assert.AreEqual(0, result3);
        }
        [TestMethod]
        public void GetPositiveValueOrZeroShouldReturnValueWhenValueIsPositive()
        {
            //Assert
            int testValue = int.MaxValue;
            int testValue2 = 1;
            //Act
            var result = CombatManager.GetPositiveValueOrZero(testValue);
            var result2 = CombatManager.GetPositiveValueOrZero(testValue2);
            //Assert
            Assert.AreEqual(testValue, result);
            Assert.AreEqual(testValue2, result2);
        }

        [TestMethod]
        public void GetPositiveModifiersForAttributesShouldReturnEmptyListWhenNoAttributeIsFound()
        {
            var modifiers = new List<AttributeModifier>
            {
                new AttributeModifier
                {
                    Attribute = CharacterAttribute.Strenght,
                    IsPositive = true,
                    Value = 20
                },
                new AttributeModifier
                {
                    Attribute = CharacterAttribute.Defense,
                    IsPositive = true,
                    Value = 20
                }                
            };

            var positiveModifiers = CombatManager.GetPositiveModifiersForAttribute(CharacterAttribute.Intelligence); //with default parameter
            var positiveModifiers2 = CombatManager.GetPositiveModifiersForAttribute(CharacterAttribute.Intelligence, modifiers);

            modifiers.Add(new AttributeModifier
            {
                Attribute = CharacterAttribute.Intelligence,
                IsPositive = false,
                Value = 20
            });
            var positiveModifiers3 = CombatManager.GetPositiveModifiersForAttribute(CharacterAttribute.Intelligence, modifiers);

            Assert.IsNotNull(positiveModifiers);
            Assert.AreEqual(0, positiveModifiers.Count);
            Assert.IsNotNull(positiveModifiers2);
            Assert.AreEqual(0, positiveModifiers2.Count);
            Assert.IsNotNull(positiveModifiers3);
            Assert.AreEqual(0, positiveModifiers3.Count);
        }

        #endregion

        #endregion
    }
}
