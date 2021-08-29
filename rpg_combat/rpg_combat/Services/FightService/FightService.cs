using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using rpg_combat.Data;
using rpg_combat.Dtos.Fight;
using rpg_combat.Models;
using rpg_combat.Services.LifeLogService;

namespace rpg_combat.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly ILogger<FightService> logger;

        public static string ErrorMessageNoCharactersFound = "No characters found for the provided Ids";
        public static string ErrorMessageNotEnoughCharacters = "Not enough characters to make a fight happen";

        //TODO: this service should receive the CharacterService and search characters by id! then use the authenticated user..
        public FightService(DataContext context, IMapper mapper, ILogger<FightService> logger)
        {
            this.mapper = mapper;
            this.context = context;
            this.logger = logger;
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

            int damage = CombatManager.DoSkillDamage(attacker, opponent, skill);

            context.Characters.Update(opponent);
            await context.SaveChangesAsync();
            return FromAttack(damage, attacker, opponent);
        }

        public async Task<ServiceResponse<List<HighscoreDto>>> GetHighscore()
        {
            //orders by victories, if they are the same for more than 1 character then orders by defeats
            var characters = await context.Characters.Where(c => c.Fights > 0)
                                                     .OrderByDescending(c => c.Victories).ThenBy(c => c.Defeats)
                                                     .ToListAsync();
            var charactersDto = characters.Select(c => mapper.Map<HighscoreDto>(c)).ToList();
            return ServiceResponse<List<HighscoreDto>>.From(charactersDto);
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var attacker = await context.Characters.Include(c => c.Weapon).FirstOrDefaultAsync(c => c.Id == request.AttackerId);
            //TODO: when update the find opponent a new method in the characterService must be created since the findById only searchs for the authenticated user.
            var opponent = await context.Characters.FirstOrDefaultAsync(c => c.Id == request.OpponentId);
            if (attacker is null || opponent is null)
                return ServiceResponse<AttackResultDto>.FailedFrom("Attacker or Opponent not found");


            //any value between zero and the strenght (plus weapon damage) - any value between zero and defense
            int damage = CombatManager.DoWeaponDamage(attacker, opponent);

            context.Characters.Update(opponent);
            await context.SaveChangesAsync();
            return FromAttack(damage, attacker, opponent);
        }
        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            List<Character> characters = await GetCharactersWithSkillsAndWeapon(request.CharacterIds);
            if (characters.Count == 0)
                return ServiceResponse<FightResultDto>.FailedFrom(ErrorMessageNoCharactersFound);
            if (characters.Count < 2)
                return ServiceResponse<FightResultDto>.FailedFrom("Not enough characters to make a fight happen");

            //defeat today considers only the first to die, not all of them.
            bool defeated = false;
            List<string> battleLog = new List<string>();
            int winnerId = request.CharacterIds.First();
            List<LifeLog> lifeLogs = new List<LifeLog>();
            while (!defeated)
            {
                foreach (Character attacker in characters)
                {
                    List<Character> opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                    var opponent = opponents[new Random().Next(opponents.Count)];

                    int damage = 0;
                    string attackUsed = String.Empty;
                    bool skipTurn = false;

                    //Todo: possibility to do nothing
                    switch (CombatManager.DefineAttackOption())
                    {
                        case CombatManager.AttackOptions.Weapon:
                            attackUsed = attacker.Weapon.Name;
                            damage = CombatManager.DoWeaponDamage(attacker, opponent);
                            break;
                        case CombatManager.AttackOptions.Skill:
                            var skill = attacker.CharacterSkills[new Random().Next(attacker.CharacterSkills.Count)].Skill;
                            attackUsed = skill.Name;
                            damage = CombatManager.DoSkillDamage(attacker, opponent, skill);
                            break;
                        case CombatManager.AttackOptions.DoNothing:
                            skipTurn = true;
                            break;
                    }
                    
                    if (skipTurn)
                    {
                        battleLog.Add($"{attacker.Name} observed the adversary for too long and lost his chance to attack!");
                    }
                    else
                    {
                        string hitDamage = damage <= 0 ? "and missed!" : $"with {damage} damage.";
                        battleLog.Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} {hitDamage}");
                    }
                    if (opponent.HitPoints <= 0)
                    {
                        winnerId = attacker.Id;
                        defeated = true;
                        attacker.Victories++;
                        opponent.Defeats++;
                        battleLog.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                        battleLog.Add($"{opponent.Name} has been defeated"!);
                        lifeLogs.Add(LifeLogExtensions.CreateVictoryLog(attacker, attackUsed, opponent.Name));
                        lifeLogs.Add(LifeLogExtensions.CreateDefeatLog(opponent, attackUsed, attacker));
                        break;
                    }
                }
            }

            characters.ForEach(c =>
            {
                c.Fights++;
                c.HitPoints = 100;
            });

            context.Characters.UpdateRange(characters);
            await context.LifeLogs.AddRangeAsync(lifeLogs);
            await context.SaveChangesAsync();
            return ServiceResponse<FightResultDto>.From(new FightResultDto
            {
                WinnerId = winnerId,
                BattleLog = battleLog
            });
        }

        private async Task<List<Character>> GetCharactersWithSkillsAndWeapon(List<int> characterIds)
        {
            return await context.Characters
                                            .Include(c => c.Weapon)
                                            .Include(c => c.CharacterSkills).ThenInclude(cs => cs.Skill)
                                            .Where(c => characterIds.Contains(c.Id)).ToListAsync();
        }

        private static ServiceResponse<AttackResultDto> FromAttack(int damage, Character attacker, Character opponent)
        {
            return ServiceResponse<AttackResultDto>.From(new AttackResultDto
            {
                Attacker = attacker.Name,
                AttackerHp = attacker.HitPoints,
                Damage = damage,
                Opponent = opponent.Name,
                OpponentHp = opponent.HitPoints
            });
        }
    }
}