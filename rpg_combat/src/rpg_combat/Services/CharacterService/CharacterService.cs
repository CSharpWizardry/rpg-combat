using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using rpg_combat.Data;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;
using rpg_combat.Services.LifeLogService;

namespace rpg_combat.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        private readonly DataContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<CharacterService> logger;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor, ILogger<CharacterService> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
            this.context = context;
            this.logger = logger;
        }
        public async Task<GetCharacterDto> Add(AddCharacterDto newCharacter)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            var character = mapper.Map<Character>(newCharacter);
            character.User = user;
            character.LifeLogs.Add(LifeLogExtensions.CreateBornLog(character));
            await context.Characters.AddAsync(character);
            await context.SaveChangesAsync();
            return mapper.Map<GetCharacterDto>(character);
        }

        public async Task Delete(int id)
        {
            var character = await context.Characters.Where(c => c.User.Id == GetUserId()).FirstAsync(c => c.Id == id);
            if (character is null)
                return;
            context.Characters.Remove(character);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<GetCharacterDto>> GetAll()
        {
            List<Character> characters = await context.Characters.Where(c => c.User.Id == GetUserId()).ToListAsync();
            return characters.Select(character => mapper.Map<GetCharacterDto>(character)).ToList();
        }

        public async Task<GetCharacterDto> GetById(int id)
        {
            Character character = await context.Characters
                                                        .Include(c => c.Weapon)                                                        
                                                        .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                                                        .Where(c => c.User.Id == GetUserId()).FirstOrDefaultAsync(c => c.Id == id);
            //logger.LogInformation($"Character with id {id} Loaded!");
            //logger.LogInformation($"Character {character.Name}, log count: {character.LifeLogs.Count}");
            return mapper.Map<GetCharacterDto>(character);
        }

        public async Task<GetCharacterDto> Update(UpdateCharacterDto updatedCharacterDto)
        {
            var character = await context.Characters
                                                    .Include(c => c.User)//makes EF include User entity on the response, otherwise it comes as null!
                                                    .FirstOrDefaultAsync(c => c.Id == updatedCharacterDto.Id);
            if (character.User?.Id != GetUserId())
            {
                //TODO: return ServiceResponse with error!
            }
            character.Name = updatedCharacterDto.Name;
            character.Class = updatedCharacterDto.Class;
            character.Defense = updatedCharacterDto.Defense;
            character.HitPoints = updatedCharacterDto.HitPoints;
            character.Intelligence = updatedCharacterDto.Intelligence;
            character.Strength = updatedCharacterDto.Strength;
            character.User = await context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
            context.Characters.Update(character);
            await context.SaveChangesAsync();
            return mapper.Map<GetCharacterDto>(character);
        }

        private int GetUserId() => int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}