using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg_combat.Dtos.Fight;
using rpg_combat.Services.FightService;

namespace rpg_combat.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService fightService;
        public FightController(IFightService fightService)
        {
            this.fightService = fightService;

        }

        [HttpPost("weapon")]
        public async Task<IActionResult> WeaponAttack(WeaponAttackDto attackRequest)
        {
            return Ok(await fightService.WeaponAttack(attackRequest));
        }

        [HttpPost("skill")]
        public async Task<IActionResult> SkillAttack(SkillAttackDto attackRequest)
        {
            return Ok(await fightService.SkillAttack(attackRequest));
        }

        //Becomes the defaul route
        [HttpPost]
        public async Task<IActionResult> Fight(FightRequestDto fightRequest)
        {
            return Ok(await fightService.Fight(fightRequest));
        }
    }
}