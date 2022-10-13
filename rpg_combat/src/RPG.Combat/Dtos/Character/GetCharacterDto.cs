using System.Collections.Generic;
using RPG.Combat.Dtos.Skill;
using RPG.Combat.Dtos.Weapon;
using RPG.Combat.Models;

namespace RPG.Combat.Dtos.Character
{
    public class GetCharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "unnamed character";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public CharacterClass Class { get; set; } = CharacterClass.Fighter;
        public GetWeaponDto Weapon { get; set; }
        public List<GetSkillDto> Skills { get; set; }
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
        public List<LifeLog> LifeLogs { get; set; }
    }
}