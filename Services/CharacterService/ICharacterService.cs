using System.Collections.Generic;
using System.Threading.Tasks;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;

namespace rpg_combat.Services.CharacterService
{
    public interface ICharacterService
    {
        Task<IEnumerable<GetCharacterDto>> GetAll();
        Task<GetCharacterDto> GetById(int id);
        Task<GetCharacterDto> Add(AddCharacterDto newCharacter);
        Task<GetCharacterDto> Update(UpdateCharacterDto updatedCharacter);
        Task Delete(int id);
    }
}