using RPG.Combat.Models;
using System;

namespace RPG.Combat.Domain
{
    public class BattleEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public AttributeModifier Effect { get; set; }
        public Frequency EventFrequency { get; set; }
        public Target EffectTarget { get; set; }
        /// <summary>
        /// Expects a trigger in the format of: 
        /// - Tuple with attacker and attack option
        /// - Opponent
        /// - returns true if it was triggered, false otherwise
        /// ((attacker, AttackOption), opponent) => return boolean
        /// </summary>
        public Func<(Character, AttackOptions), Character, bool> Trigger { get; set; }
        public bool CanStack { get; set; }
    }
}
