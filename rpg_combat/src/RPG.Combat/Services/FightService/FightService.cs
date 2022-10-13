using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RPG.Combat.Data;
using RPG.Combat.Domain;
using RPG.Combat.Dtos.Fight;
using RPG.Combat.Models;
using RPG.Combat.Services.LifeLogService;

namespace RPG.Combat.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;
        private readonly ILogger<FightService> logger;
        private static readonly List<BattleEvent> events = GenerateEvents();

        public static readonly string ErrorMessageNoCharactersFound = "No characters found for the provided Ids";
        public static readonly string ErrorMessageNotEnoughCharacters = "Not enough characters to make a fight happen";

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

        //TODO: CREATE A COMBAT PIPELINE!
        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            List<Character> characters = await GetCharactersWithSkillsAndWeapon(request.CharacterIds);
            if (characters.Count == 0)
                return ServiceResponse<FightResultDto>.FailedFrom(ErrorMessageNoCharactersFound);
            if (characters.Count < 2)
                return ServiceResponse<FightResultDto>.FailedFrom("Not enough characters to make a fight happen");

            bool defeated = false;
            int opponentsStillStanding = characters.Count - 1;
            Dictionary<Character, bool> opponentsDefeated = characters.ToDictionary(c => c, _ => false);
            List<string> battleLog = new List<string>();
            int winnerId = request.CharacterIds.First();
            List<LifeLog> lifeLogs = new List<LifeLog>();
            while (!defeated)
            {
                foreach (Character attacker in characters)
                {
                    if (opponentsDefeated[attacker])
                    {
                        continue;
                    }
                    List<Character> opponents = opponentsDefeated.Where(keyValuePair => !keyValuePair.Value && keyValuePair.Key.Id != attacker.Id).Select(kvp => kvp.Key).ToList();
                    var opponent = opponents[new Random().Next(opponents.Count)];

                    int damage = 0;
                    string attackUsed = String.Empty;
                    bool skipTurn = false;
                    AttackOptions attackSelected = CombatManager.DefineAttackOption();
                    switch (attackSelected)
                    {
                        case AttackOptions.Weapon:
                            attackUsed = attacker.Weapon.Name;
                            damage = CombatManager.DoWeaponDamage(attacker, opponent);
                            break;
                        case AttackOptions.Skill:
                            var skill = attacker.CharacterSkills[new Random().Next(attacker.CharacterSkills.Count)].Skill;
                            attackUsed = skill.Name;
                            damage = CombatManager.DoSkillDamage(attacker, opponent, skill);
                            break;
                        case AttackOptions.DoNothing:
                            skipTurn = true;
                            break;
                    }

                    RunEvents(battleLog, attacker, opponent, attackSelected);

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
                        opponentsDefeated[opponent] = true;
                        opponent.Defeats++;
                        battleLog.Add($"{opponent.Name} has been defeated"!);
                        lifeLogs.Add(LifeLogExtensions.CreateDefeatLog(opponent, attackUsed, attacker));
                        opponentsStillStanding--;
                    }
                    if (opponentsStillStanding == 0)
                    {
                        winnerId = attacker.Id;
                        attacker.Victories++;
                        battleLog.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                        lifeLogs.Add(LifeLogExtensions.CreateVictoryLog(attacker, attackUsed, opponent.Name));
                        defeated = true;
                        break;
                    }
                }
            }

            characters.ForEach(c =>
            {
                c.Fights++;
                c.HitPoints = 100;
                //cleans all non-permanent modifiers added during a fight
                c.AttributeModifiers.RemoveAll(modifier => !modifier.IsPermanent);
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

        private static void RunEvents(List<string> battleLog, Character attacker, Character opponent, AttackOptions attackSelected)
        {
            foreach (var battleEvent in events)
            {
                var wasTrigged = battleEvent.Trigger((attacker, attackSelected), opponent);
                if (wasTrigged && battleEvent.EffectTarget.Equals(Target.OPPONENT))
                {
                    if (battleEvent.CanStack || !opponent.AttributeModifiers.Contains(battleEvent.Effect))
                    {
                        opponent.AttributeModifiers.Add(battleEvent.Effect);
                        battleLog.Add($"{opponent.Name} received the effect {battleEvent.Name}");
                        if (battleEvent.Effect.IsPermanent)
                            opponent.LifeLogs.Add(LifeLogExtensions.CreateAttributeModifierLog(opponent, battleEvent.Effect));
                    }
                    else
                    {
                        battleLog.Add($"{opponent.Name} Didn't receive the effect {battleEvent.Name} because he already has it");
                    }
                }
            }
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


        //TODO: Temporary only, should come from DB or a file
        private static List<BattleEvent> GenerateEvents()
        {
            return new List<BattleEvent> {
                new BattleEvent{
                    Name = "Pregador",
                    Description = "Um forte golpe de martelo na cabeça",
                    Effect = new AttributeModifier(CharacterAttribute.Intelligence, false, 2),
                    EffectTarget = Target.OPPONENT,
                    EventFrequency = Frequency.AllTheTime,
                    Trigger = ((Character self, AttackOptions attackOption) characterAttackTuple, Character opponent) =>
                    {
                        bool containsHammer = characterAttackTuple.self.Weapon.Name.Contains("Spear", StringComparison.InvariantCultureIgnoreCase);
                        return characterAttackTuple.attackOption.Equals(AttackOptions.Weapon) && containsHammer;
                    }
                },
                new BattleEvent{
                    Name = "Manetinha",
                    Description = "Virei um membro dos cavaleiros negros",
                    //Modificador cortar na metade
                    Effect = new AttributeModifier {
                        Attribute = CharacterAttribute.Strenght,
                        Name = "Desmembrado",
                        Description = "Character lost a limb",
                        Origin = "Manetinha",
                        IsPositive = false,
                        Value = 10,
                        IsPermanent = true
                    },
                    EffectTarget = Target.OPPONENT,
                    EventFrequency = Frequency.AllTheTime,
                    Trigger = ((Character self, AttackOptions attackOption) characterAttackTuple, Character opponent) =>
                    {
                        return opponent.Strength < 20 && opponent.HitPoints < 30;
                    }
                },
                new BattleEvent{
                    Name = "Pombo cagou na minha cabeça e me cegou",
                    Description = "Peguem o pombo",
                    Effect = new AttributeModifier(CharacterAttribute.Defense, false, 12),
                    EffectTarget = Target.SELF,
                    EventFrequency = Frequency.AllTheTime,
                    Trigger = ((Character self, AttackOptions attackOption) characterAttackTuple, Character opponent) =>
                    {
                        var rand = new Random();
                        return rand.Next(30) == 15;
                    }
                },
            };
        }

    }
}