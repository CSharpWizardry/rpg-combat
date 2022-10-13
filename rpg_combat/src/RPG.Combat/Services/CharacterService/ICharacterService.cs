using System.Collections.Generic;
using System.Threading.Tasks;
using RPG.Combat.Dtos.Character;
using RPG.Combat.Models;

namespace RPG.Combat.Services.CharacterService
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