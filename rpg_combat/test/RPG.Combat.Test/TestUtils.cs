using RPG.Combat.Dtos.Character;
using RPG.Combat.Models;
using RPG.Combat.Services.WeaponService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPG.Combat.Test
{
    public static class TestUtils
    {
        private static int characterId = 1;
        public static Character CreateCharacterWithUser(int characterId, int userId)
            => new Character
            {
                Id = characterId,
                Class = CharacterClass.Wizard,
                HitPoints = 100,
                User = new User
                {
                    Id = userId,
                    Username = $"User {userId}"
                }
            };

        public static Character CreateCharacter(int id) => new Character { Id = id, Name = $"character {id}", Class = CharacterClass.Fighter, HitPoints = 100 };

        public static Character CreateCharacterWithWeaponAndSkills()
        {
            characterId++;
            var character = CreateCharacter(characterId);
            var weaponDto = WeaponFactory.GetWeaponForClass(character.Class, characterId);
            character.Weapon = new Weapon
            {
                Name = weaponDto.Name,
                Damage = weaponDto.Damage
            };
            var skills = GetStarterSkills();
            character.CharacterSkills.Add(new CharacterSkill { CharacterId = characterId, SkillId = 1, Skill = skills.Single(s => s.Id == 1) });
            character.CharacterSkills.Add(new CharacterSkill { CharacterId = characterId, SkillId = 2, Skill = skills.Single(s => s.Id == 2) });
            character.CharacterSkills.Add(new CharacterSkill { CharacterId = characterId, SkillId = 3, Skill = skills.Single(s => s.Id == 3) });
            return character;
        }

        public static List<Skill> GetStarterSkills()
        {
            return new List<Skill>
            {
                new Skill { Id = 1, Damage = 15, Name = "Frenzy" },
                new Skill { Id = 2, Damage = 20, Name = "Blizzard" },
                new Skill { Id = 3, Damage = 11, Name = "Radiant Spear" }
            };
        }

        public static GetCharacterDto CreateGetCharacterDto(int id) => new GetCharacterDto { Id = id, Name = $"character {id}" };

        public static IEnumerable<GetCharacterDto> CreateFakeCharacterDtoList(int count = 10)
        {
            var list = new List<GetCharacterDto>();
            for (int i = 0; i < count; i++)
                list.Add(CreateGetCharacterDto(i));
            return list;
        }
    }
}
