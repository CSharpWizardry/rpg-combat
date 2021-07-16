using Microsoft.AspNetCore.Mvc;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;
using rpg_combat.Services.CharacterService;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_combat.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService characterService;

        public CharacterController(ICharacterService characterService)
        {
            this.characterService = characterService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            return Ok(await characterService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            var character = await characterService.GetById(id);
            if (character is null)
                return NotFound();
            
            return Ok(character);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddCharacterDto character)
        {
            await characterService.Add(character);
            return CreatedAtAction(nameof(Create), new { }, character);
        }

    }
}