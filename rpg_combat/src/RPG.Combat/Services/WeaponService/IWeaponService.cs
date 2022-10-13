using System.Threading.Tasks;
using RPG.Combat.Dtos.Character;
using RPG.Combat.Dtos.Weapon;
using RPG.Combat.Models;

namespace RPG.Combat.Services.WeaponService
{
    public interface IWeaponService
    {
         Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon);
    }
}