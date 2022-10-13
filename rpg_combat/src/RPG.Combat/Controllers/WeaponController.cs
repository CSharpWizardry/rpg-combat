using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPG.Combat.Dtos.Weapon;
using RPG.Combat.Services.WeaponService;

namespace RPG.Combat.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService weaponService;
        public WeaponController(IWeaponService weaponService)
        {
            this.weaponService = weaponService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddWeaponDto newWeapon)
        {
            var response  = await weaponService.AddWeapon(newWeapon);
            if (!response.Success)
                return BadRequest(response.Message);
            
            return Ok(response.Data);
        }
    }
}