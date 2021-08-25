using System;

namespace rpg_combat.Models
{
    public class LifeLog
    {
        public int Id { get; set; }
        public string Log { get; set; }
        public DateTime HappenedOn { get; set; }
        public bool IsBattleLog { get; set; }
        public bool IsVictory { get; set; }
        public Character Character { get; set; }

        //public static LifeLog CreateVictoryLog(Character character, string attackUsed, string opponentName)
        //{
        //    return new LifeLog
        //    {
        //        Character = character,
        //        HappenedOn = DateTime.UtcNow,
        //        Log = $"On {DateTime.UtcNow} {character.Name} won his {character.Victories}th victory. He used {attackUsed} to hit the final blow and left the battle field with {character.HitPoints} hp. His opponent was {opponentName}.",
        //        IsBattleLog = true,
        //        IsVictory = true
        //    };
        //}

        public static LifeLog CreateDefeatLog(Character character, string attackUsed, Character opponent)
        {
            return new LifeLog
            {
                Character = character,
                HappenedOn = DateTime.UtcNow,
                Log = $"On {DateTime.UtcNow} {character.Name} lost his {character.Defeats}th fight. His opponent was {opponent.Name} and he used {attackUsed} to hit the final blow, leaving the battle field with {opponent.HitPoints} hp.",
                IsBattleLog = true,
                IsVictory = false
            };
        }

        public static LifeLog CreateBornLog(Character character)
        {
            return new LifeLog
            {
                Character = character,
                HappenedOn = DateTime.UtcNow,
                Log = $"On {DateTime.UtcNow} {character.Name} was born as a(n) {character.Class}. At the time he had {character.Intelligence} intelligence, {character.Strength} strength.",
                IsBattleLog = false,
                IsVictory = false
            };
        }
    }
}
