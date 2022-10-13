
using System.Collections.Generic;
using System.Threading.Tasks;
using RPG.Combat.Dtos;
using RPG.Combat.Dtos.Character;
using RPG.Combat.Dtos.Skill;
using RPG.Combat.Models;

namespace RPG.Combat.Services.CharacterSkillService
{
    public interface ICharacterSkillService
    {
         Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill);
        Task<ServiceResponse<List<GetSkillDto>>> GetSkills();
    }
}