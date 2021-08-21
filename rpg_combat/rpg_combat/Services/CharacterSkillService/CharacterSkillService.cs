using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg_combat.Data;
using rpg_combat.Dtos;
using rpg_combat.Dtos.Character;
using rpg_combat.Dtos.Skill;
using rpg_combat.Models;

namespace rpg_combat.Services.CharacterSkillService
{
    public class CharacterSkillService : ICharacterSkillService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public CharacterSkillService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }
        public async Task<ServiceResponse<GetCharacterDto>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            try 
            {
                var character = await context.Characters
                                                    .Include(c => c.Weapon)
                                                    //includes the skills that exist inside the characterSkills relationship in the response
                                                    .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                                                    .FirstOrDefaultAsync(c => c.Id == newCharacterSkill.CharacterId && c.User.Id == GetUserId());
                if (character is null)
                    return ServiceResponse<GetCharacterDto>.FailedFrom("Character not found");
                
                var skill = await context.Skills.FirstOrDefaultAsync(s => s.Id == newCharacterSkill.SkillId);
                if (skill is null)
                    return ServiceResponse<GetCharacterDto>.FailedFrom("Skill not found");
                
                CharacterSkill characterSkill = new CharacterSkill
                {
                    Skill = skill,
                    Character = character
                };
                await context.CharacterSkills.AddAsync(characterSkill);
                await context.SaveChangesAsync();

                return ServiceResponse<GetCharacterDto>.From(mapper.Map<GetCharacterDto>(character));
            }
            catch(Exception ex)
            {
                return ServiceResponse<GetCharacterDto>.FailedFrom(ex.Message);
            }
        }

        public async Task<ServiceResponse<List<GetSkillDto>>> GetSkills()
        {
            try
            {
                var skills = await context.Skills.ToListAsync();
                var skillsDto = skills.Select(s => mapper.Map<GetSkillDto>(s)).ToList();
                return ServiceResponse<List<GetSkillDto>>.From(skillsDto);
            }
            catch(Exception ex)
            {
                return ServiceResponse<List<GetSkillDto>>.FailedFrom(ex.Message);
            }
        }

        private int GetUserId() => int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}