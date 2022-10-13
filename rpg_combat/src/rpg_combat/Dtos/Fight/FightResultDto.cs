using System.Collections.Generic;

namespace rpg_combat.Dtos.Fight
{
    public class FightResultDto
    {
        public int WinnerId { get; set; }
        public List<string> BattleLog { get; set; }
    }
}