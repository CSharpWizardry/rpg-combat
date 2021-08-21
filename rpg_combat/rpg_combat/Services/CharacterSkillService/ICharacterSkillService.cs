
using System.Collections.Generic;
using System.Threading.Tasks;
using rpg_combat.Dtos;
using rpg_combat.Dtos.Character;
using rpg_combat.Dtos.Skill;
using rpg_combat.Models;

namespace rpg_combat.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
         Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
        Task<ServiceResponse<List<GetSkillDto>>> GetSkills();
    }
}