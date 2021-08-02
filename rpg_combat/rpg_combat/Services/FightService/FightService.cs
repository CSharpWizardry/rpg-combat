using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rpg_combat.Data;
using rpg_combat.Dtos.Fight;
using rpg_combat.Models;

namespace rpg_combat.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext context;
        //TODO: this service should receive the CharacterService and search characters by id! then use the authenticated user..
        public FightService(DataContext context)
        {
            this.context = context;

        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var attacker = await context.Characters
                                                    .Include(c => c.CharacterSkills)
                                                    .ThenInclude(cs => cs.Skill)
                                                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
            var opponent = await context.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);
            if (attacker is null || opponent is null)
                return ServiceResponse<AttackResultDto>.FailedFrom("Attacker or Opponent not found");

             var skill = attacker.CharacterSkills.FirstOrDefault(s => s.SkillId == request.SkillId)?.Skill;
             if (skill is null)
                return ServiceResponse<AttackResultDto>.FailedFrom($"{attacker.Name} doesn't know that skill");
            
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
             damage -= new Random().Next(opponent.Defense);

             if (damage > 0)
            {
                opponent.HitPoints -= damage;
                context.Characters.Update(opponent);
                await context.SaveChangesAsync();
            }
            
            return FromAttack(damage, attacker, opponent);
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var attacker = await context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.AttackerId);
            //TODO: when update the find opponent a new method in the characterService must be created since the findById only searchs for the authenticated user.
            var opponent = await context.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);
            if (attacker is null || opponent is null)
                return ServiceResponse<AttackResultDto>.FailedFrom("Attacker or Opponent not found");
            
            
            //any value between zero and the strenght (plus weapon damage) - any value between zero and defense
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
                context.Characters.Update(opponent);
                await context.SaveChangesAsync();
            }
            
            return FromAttack(damage, attacker, opponent);
        }

        private static ServiceResponse<AttackResultDto> FromAttack(int damage, Character attacker, Character opponent)
        {
            return ServiceResponse<AttackResultDto>.From(new AttackResultDto {
                Attacker = attacker.Name,
                AttackerHp = attacker.HitPoints,
                Damage = damage,
                Opponent = opponent.Name,
                OpponentHp = opponent.HitPoints
            });
        }
    }
}