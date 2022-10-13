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
    }
}
