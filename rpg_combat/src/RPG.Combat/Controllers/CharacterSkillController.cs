using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPG.Combat.Dtos;
using RPG.Combat.Dtos.Skill;
using RPG.Combat.Services.CharacterSkillService;

namespace RPG.Combat.Controllers
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

        [HttpGet]
        public async Task<ActionResult<List<GetSkillDto>>> GetSkills()
        {
            var serviceResponse = await characterSkillService.GetSkills();
            if (serviceResponse.Success)
                return Ok(serviceResponse.Data);

            return BadRequest(serviceResponse.Message);

        }
    }
}