using System.Linq;
using AutoMapper;
using RPG.Combat.Dtos.Character;
using RPG.Combat.Dtos.Fight;
using RPG.Combat.Dtos.Skill;
using RPG.Combat.Dtos.Weapon;
using RPG.Combat.Models;

namespace RPG.Combat
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
            CreateMap<Character, HighscoreDto>();
            CreateMap<LifeLog, GetLifeLogDto>();
        }
    }
}