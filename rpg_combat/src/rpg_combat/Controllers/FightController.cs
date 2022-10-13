using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Simulates a fight of all x all for the provided character ids.
        /// </summary>
        /// <param name="fightRequest"></param>
        /// <returns>A battle log with descriptions of each turn and the winner's id</returns>
        /// <response code="200">Returns fight result</response>
        /// <response code="404">No characters found for the provided Ids</response>
        /// <response code="400">Possible not enough characters to make a fight happen </response>
        //Becomes the defaul route
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Fight(FightRequestDto fightRequest)
        {
            var serviceResponse = await fightService.Fight(fightRequest);
            if (serviceResponse.Success)
                return Ok(serviceResponse);
            if (serviceResponse.Message.Equals(FightService.ErrorMessageNoCharactersFound))
                return NotFound(serviceResponse.Message);
            if (serviceResponse.Message.Equals(FightService.ErrorMessageNotEnoughCharacters))
                return BadRequest(serviceResponse.Message);

            return BadRequest();
        }

        /// <summary>
        /// provides the highscore of all characters, not only the logged-in user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHighscore()
        {
            return Ok(await fightService.GetHighscore());
        }
    }
}