using System.Collections.Generic;

namespace rpg_combat.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "unnamed character";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public CharacterClass Class { get; set; } = CharacterClass.Fighter;
        public User User { get; set; }
        public Weapon Weapon { get; set; }
        public List<CharacterSkill> CharacterSkills { get; set; } = new List<CharacterSkill>();
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
        public List<LifeLog> LifeLogs { get; set; } = new List<LifeLog>();
        public List<Modifier> Modifiers { get; set; } = new List<Modifier>();
    }
}