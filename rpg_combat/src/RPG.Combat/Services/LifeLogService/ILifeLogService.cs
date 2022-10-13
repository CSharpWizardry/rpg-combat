using RPG.Combat.Dtos.Character;
using RPG.Combat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RPG.Combat.Services.LifeLogService
{
    public interface ILifeLogService
    {
        Task<ServiceResponse<List<GetLifeLogDto>>> FromCharacter(int characterId);
        Task Add(LifeLog log);
        Task Add(List<LifeLog> logs);
    }
}
