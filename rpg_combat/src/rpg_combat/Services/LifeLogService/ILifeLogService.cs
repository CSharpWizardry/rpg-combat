using rpg_combat.Dtos.Character;
using rpg_combat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rpg_combat.Services.LifeLogService
{
    public interface ILifeLogService
    {
        Task<ServiceResponse<List<GetLifeLogDto>>> FromCharacter(int characterId);
        Task Add(LifeLog log);
        Task Add(List<LifeLog> logs);
    }
}
