using System.Threading.Tasks;
using rpg_combat.Dtos.Character;
using rpg_combat.Dtos.Weapon;
using rpg_combat.Models;

namespace rpg_combat.Services.WeaponService
{
    public interface IWeaponService
    {
         Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}