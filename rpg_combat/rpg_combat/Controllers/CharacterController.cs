using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rpg_combat.Dtos.Character;
using rpg_combat.Services.CharacterService;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace rpg_combat.Controllers
{    
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService characterService;

        public CharacterController(ICharacterService characterService)
        {
            this.characterService = characterService;
        }

        /// <summary>
        /// Returns a list with all characters.
        /// </summary>
        /// <response code="200">Returns list with all characters</response>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetCharacterDto>>> Get()
        {
            return Ok(await characterService.GetAll());
        }

        
        /// <summary>
        /// Gets information on a specific character.
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Returns character that matches the provided Id</response>
        /// <response code="404">No character found for the provided Id</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetCharacterDto>> GetSingle(int id)
        {
            var character = await characterService.GetById(id);
            if (character is null)
                return NotFound();
            
            return Ok(character);
        }


        /// <summary>
        /// Creates a Character.
        /// </summary>
        /// <response code="201">Returns the newly created character</response>
        /// <response code="400">If the character request is null</response>    
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(AddCharacterDto character)
        {
            var createdCharacter = await characterService.Add(character);
            return CreatedAtAction(nameof(Create), new { id = createdCharacter.Id}, character);
        }

        /// <summary>
        /// Updates a specific Character by its Id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /character/2
        ///     {
        ///        "id": 2,
        ///        "name": "New Characters name",
        ///        "Class": 1
        ///     }
        /// Note that all character's properties will be updated, even if not provided in the payload
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="character"></param>
        /// <response code="204">Character successfully updated</response>
        /// <response code="400">Invalid payload/Id</response>
        /// <response code="404">No character found for the provided Id</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UpdateCharacterDto character)
        {
            if (id != character.Id)
                return BadRequest();

            var existingCharacter = await characterService.GetById(id);
            if (existingCharacter is null)
                return NotFound();
            
            await characterService.Update(character);
            return NoContent();
        }

        /// <summary>
        /// Deletes a specific Character by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <response code="204">Character successfully deleted</response>
        /// <response code="404">No character found for the provided Id</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existingCharacter = await characterService.GetById(id);
            if (existingCharacter is null)
                return NotFound();

            await characterService.Delete(id);
            return NoContent();
        }

    }
}