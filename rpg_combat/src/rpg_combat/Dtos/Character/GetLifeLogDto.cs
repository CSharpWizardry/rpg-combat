
using System;

namespace rpg_combat.Dtos.Character
{
    public class GetLifeLogDto
    {
        public string Log { get; set; }
        public DateTime HappenedOn { get; set; }
        public bool IsBattleLog { get; set; }
    }
}
