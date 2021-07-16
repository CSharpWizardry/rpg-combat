using Microsoft.AspNetCore.Mvc;
using rpg_combat.Models;
using System.Collections.Generic;
using System.Linq;

namespace rpg_combat.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private static List<Character> characters = new List<Character>{
            new Character(),
            new Character {Id = 1, Name = "character 2", Class = CharacterClass.Wizard},
            new Character {Id = 2, Name = "character 3", Class = CharacterClass.Cleric},
        };

        [HttpGet("GetAll")]
        public IActionResult Get()
        {
            return Ok(characters);
        }

        [HttpGet("{id}")]
        public IActionResult GetSingle(int id)
        {
            var character = characters.FirstOrDefault(c => c.Id == id);
            if (character is null)
                return NotFound();
            
            return Ok(character);
        }

    }
}