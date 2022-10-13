using System.Collections.Generic;
using System.Threading.Tasks;
using RPG.Combat.Dtos.Fight;
using RPG.Combat.Models;

namespace RPG.Combat.Services.FightService
{
    public interface IFightService
    {
         Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request);
         Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request);
         Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request);
         Task<ServiceResponse<List<HighscoreDto>>> GetHighscore();
    }
}