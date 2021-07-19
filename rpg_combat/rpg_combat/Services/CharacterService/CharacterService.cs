using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg_combat.Data;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;

namespace rpg_combat.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {        
        private readonly IMapper mapper;
        private readonly DataContext context;

        public CharacterService(IMapper mapper, DataContext context) 
        {
            this.mapper = mapper;
            this.context = context;
        }
        public async Task<GetCharacterDto> Add(AddCharacterDto newCharacter)
        {
            var character = mapper.Map<Character>(newCharacter);
            await context.Characters.AddAsync(character);
            await context.SaveChangesAsync();
            return mapper.Map<GetCharacterDto>(character);
        }

        public async Task Delete(int id)
        {
            var character = await context.Characters.FirstAsync(c => c.Id == id);
            context.Characters.Remove(character);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetCharacterDto>> GetAll()
        {
            List<Character> characters = await context.Characters.ToListAsync();
            return characters.Select(character => mapper.Map<GetCharacterDto>(character)).ToList();
        }

        public async Task<GetCharacterDto> GetById(int id)
        {
            Character character = await context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            return mapper.Map<GetCharacterDto>(character);
        }

        public async Task<GetCharacterDto> Update(UpdateCharacterDto updatedCharacterDto)
        {
            var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacterDto.Id);
            character.Name = updatedCharacterDto.Name;
            character.Class = updatedCharacterDto.Class;
            character.Defense = updatedCharacterDto.Defense;
            character.HitPoints = updatedCharacterDto.HitPoints;
            character.Intelligence = updatedCharacterDto.Intelligence;
            character.Strength = updatedCharacterDto.Strength;
            context.Characters.Update(character);
            await context.SaveChangesAsync();
            return mapper.Map<GetCharacterDto>(character);
        }
    }
}