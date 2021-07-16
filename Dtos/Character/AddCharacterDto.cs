using rpg_combat.Models;

namespace rpg_combat.Dtos.Character
{
    public class AddCharacterDto
    {
        public string Name { get; set; } = "unnamed character";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defense { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public CharacterClass Class { get; set; } = CharacterClass.Fighter;
    }
}