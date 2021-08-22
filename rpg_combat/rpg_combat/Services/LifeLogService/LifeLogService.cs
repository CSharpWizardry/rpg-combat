using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg_combat.Data;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rpg_combat.Services.LifeLogService
{
    public class LifeLogService : ILifeLogService
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public LifeLogService(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task Add(LifeLog log)
        {
            await context.LifeLogs.AddAsync(log);
            await context.SaveChangesAsync();
        }

        public async Task Add(List<LifeLog> logs)
        {
            await context.LifeLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();
        }

        public async Task<ServiceResponse<List<GetLifeLogDto>>> FromCharacter(int characterId)
        {
            var lifeLogs = await context.LifeLogs.Where(l => l.Character.Id == characterId).ToListAsync();
            if (lifeLogs.Count > 0)
                return ServiceResponse<List<GetLifeLogDto>>.From(lifeLogs.Select(l => mapper.Map<GetLifeLogDto>(l)).ToList());

            return ServiceResponse<List<GetLifeLogDto>>.FailedFrom($"No life logs found for character with id {characterId}");
        }
    }
}
