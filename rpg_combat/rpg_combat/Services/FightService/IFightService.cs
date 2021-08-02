using System.Threading.Tasks;
using rpg_combat.Dtos.Fight;
using rpg_combat.Models;

namespace rpg_combat.Services.FightService
{
    public interface IFightService
    {
         Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request);
         Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request);
         Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request);
    }
}