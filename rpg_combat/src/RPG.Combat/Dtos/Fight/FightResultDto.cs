using System.Collections.Generic;

namespace RPG.Combat.Dtos.Fight
{
    public class FightResultDto
    {
        public int WinnerId { get; set; }
        public List<string> BattleLog { get; set; }
    }
}