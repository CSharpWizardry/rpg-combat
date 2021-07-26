using System.Threading.Tasks;
using rpg_combat.Dtos.Fight;
using rpg_combat.Models;

namespace rpg_combat.Services.FightService
{
    public interface IFightService
    {
         Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request);
    }
}