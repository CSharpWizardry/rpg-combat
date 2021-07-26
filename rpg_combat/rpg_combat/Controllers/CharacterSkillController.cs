using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rpg_combat.Dtos;
using rpg_combat.Services.CharacterSkillService;

namespace rpg_combat.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterSkillController : ControllerBase
    {
        private readonly ICharacterSkillService characterSkillService;
        public CharacterSkillController(ICharacterSkillService characterSkillService)
        {
            this.characterSkillService = characterSkillService;
        }

        [HttpPost]
        public async Task<IActionResult> AddCharacterSkill(AddCharacterSkillDto newCharacterSkillDto)
        {
            var serviceResponse = await characterSkillService.AddCharacterSkill(newCharacterSkillDto);
            if (serviceResponse.Success)
                return Ok(serviceResponse.Data);
            return BadRequest(serviceResponse);
        }
    }
}