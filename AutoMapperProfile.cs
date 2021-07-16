using AutoMapper;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;

namespace rpg_combat
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
        }
    }
}