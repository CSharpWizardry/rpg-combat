using RPG.Combat.Dtos.Character;
using RPG.Combat.Models;
using System;

namespace RPG.Combat.Services.LifeLogService
{
    public static class LifeLogExtensions
    {
        public static LifeLog CreateVictoryLog(Character character, string attackUsed, string opponentName)
        {
            return new LifeLog
            {
                Character = character,
                HappenedOn = DateTime.UtcNow,
                Log = $"On {DateTime.UtcNow} {character.Name} won his {character.Victories}th victory. He used {attackUsed} to hit the final blow and left the battle field with {character.HitPoints} hp. His opponent was {opponentName}.",
                IsBattleLog = true,
                IsVictory = true
            };
        }

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

        public static LifeLog CreateAttributeModifierLog(Character character, AttributeModifier modifier)
        {
            string modifierType = modifier.IsPermanent ? "buff" : "debuff";
            string signal = modifier.IsPositive ? "+" : "-";

            return new LifeLog
            {
                Character = character,
                HappenedOn = DateTime.UtcNow,
                Log = $"On {DateTime.UtcNow} {character.Name} received the {modifierType} {modifier.Name} from {modifier.Origin}. From now on his {modifier.Attribute} attribute will be considered with a {signal}{modifier.Value}",
                IsBattleLog = false,
                IsVictory = false
            };
        }

        public static GetLifeLogDto ConvertToDto(this LifeLog lifeLog)
        {
            return new GetLifeLogDto
            {
                HappenedOn = lifeLog.HappenedOn,
                IsBattleLog = lifeLog.IsBattleLog,
                Log = lifeLog.Log
            };
        }
    }
}
