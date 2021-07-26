using System.Linq;
using AutoMapper;
using rpg_combat.Dtos.Character;
using rpg_combat.Dtos.Skill;
using rpg_combat.Dtos.Weapon;
using rpg_combat.Models;

namespace rpg_combat
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>()
                .ForMember(dto => dto.Skills, c => c.MapFrom(c => c.CharacterSkills.Select(cs => cs.Skill)));
            CreateMap<AddCharacterDto, Character>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
        }
    }
}