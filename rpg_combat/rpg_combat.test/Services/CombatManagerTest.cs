using Microsoft.VisualStudio.TestTools.UnitTesting;
using rpg_combat.Models;
using rpg_combat.Services.FightService;

namespace rpg_combat.test.Services
{
    [TestClass]
    public class CombatManagerTest
    {
        private Character attacker;
        private Character opponent;

        #region Damage tests
        #region Weapon
        [TestMethod]
        public void DoWeaponDamageShouldReturnZeroWhenDefenseIsGreaterThanAttackValue()
        {
            //Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 100;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.IsTrue(damage <= 0);
        }

        [TestMethod]
        public void DoWeaponDamageShouldReturnDamageValueWhenAttackValueIsGreaterThanDefense()
        {
            //Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = 20;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 10;
            //Act
            var damage = CombatManager.DoWeaponDamage(attacker, opponent);
            //Assert
            Assert.IsTrue(damage > 0);
        }

        [TestMethod]
        public void DoWeaponDamageShouldReduceOpponentsLifeWhenAttackValueIsGreaterThanDefense()
        {
            //Arrange
            attacker = TestUtils.CreateCharacterWithWeaponAndSkills();
            attacker.Weapon.Damage = 20;
            opponent = TestUtils.CreateCharacterWithWeaponAndSkills();
            opponent.Defense = 10;
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
            int opponentsDefense = 10;
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
            Assert.IsTrue(opponent.HitPoints < maxHitPoints && opponent.HitPoints > 80);
            Assert.IsTrue(damage >= opponentsDefense  && damage < maxDamage);
        }

        [TestMethod]
        public void DoWeaponDamageShouldIgnoreNegativeWeaponDamageValue()
        {
            //Arrange
            int maxDamage = 20;
            int opponentsDefense = 10;
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
            Assert.IsTrue(opponent.HitPoints < maxHitPoints && opponent.HitPoints > maxDamage);
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
        #endregion

        #endregion
    }
}
