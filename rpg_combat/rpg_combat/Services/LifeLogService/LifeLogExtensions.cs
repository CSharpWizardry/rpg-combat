using rpg_combat.Dtos.Character;
using rpg_combat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_combat.Services.LifeLogService
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
