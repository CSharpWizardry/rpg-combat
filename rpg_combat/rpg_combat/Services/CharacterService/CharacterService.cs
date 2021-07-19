using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;

namespace rpg_combat.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character {Id = 1, Name = "character 2", Class = CharacterClass.Wizard},
            new Character {Id = 2, Name = "character 3", Class = CharacterClass.Cleric},
        };
        private readonly IMapper mapper;

        public CharacterService(IMapper mapper) 
        {
            this.mapper = mapper;
        }
        public async Task<GetCharacterDto> Add(AddCharacterDto newCharacter)
        {
            var character = mapper.Map<Character>(newCharacter);
            character.Id = characters.Max(c => c.Id) + 1;
            characters.Add(character);
            return mapper.Map<GetCharacterDto>(character);
        }

        public async Task Delete(int id)
        {
            var character = characters.FirstOrDefault(c => c.Id == id);
            characters.Remove(character);
        }

        public async Task<IEnumerable<GetCharacterDto>> GetAll()
        {
            return characters.Select(character => mapper.Map<GetCharacterDto>(character)).ToList();
        }

        public async Task<GetCharacterDto> GetById(int id)
        {
            return mapper.Map<GetCharacterDto>(characters.FirstOrDefault(c => c.Id == id));
        }

        public async Task<GetCharacterDto> Update(UpdateCharacterDto updatedCharacterDto)
        {
            var character = characters.FirstOrDefault(c => c.Id == updatedCharacterDto.Id);
            character.Name = updatedCharacterDto.Name;
            character.Class = updatedCharacterDto.Class;
            character.Defense = updatedCharacterDto.Defense;
            character.HitPoints = updatedCharacterDto.HitPoints;
            character.Intelligence = updatedCharacterDto.Intelligence;
            character.Strength = updatedCharacterDto.Strength;

            return mapper.Map<GetCharacterDto>(character);
        }
    }
}