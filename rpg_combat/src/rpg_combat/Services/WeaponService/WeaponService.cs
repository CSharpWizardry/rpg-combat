using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rpg_combat.Data;
using rpg_combat.Dtos.Character;
using rpg_combat.Dtos.Weapon;
using rpg_combat.Models;

namespace rpg_combat.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DataContext context;        
        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;

        }
        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto newWeapon)
        {
            try{
                var character = await context.Characters.FirstOrDefaultAsync(c => c.Id == newWeapon.CharacterId && c.User.Id == GetUserId());
                if (character is null)
                    return ServiceResponse<GetCharacterDto>.FailedFrom("Character not found");
                
                var weapon = new Weapon
                {
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    Character = character
                };
                
                await context.Weapons.AddAsync(weapon);
                await context.SaveChangesAsync();
                
                return ServiceResponse<GetCharacterDto>.From(mapper.Map<GetCharacterDto>(character));
            } catch(Exception exception)
            {
                return ServiceResponse<GetCharacterDto>.FailedFrom(exception.Message);
            }
        }
        private int GetUserId() => int.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}