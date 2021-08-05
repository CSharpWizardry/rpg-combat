using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using rpg_combat.Data;
using rpg_combat.Models;
using rpg_combat.Services.CharacterService;

namespace rpg_combat.test.Services
{
    public class CharacterServiceTest
    {

        private ICharacterService characterService;
        private DataContext context;

        [TestInitialize()]
        public void Initialize() 
        {
            var mapper = Substitute.For<IMapper>();
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            context = Substitute.For<DataContext>();
            characterService = new CharacterService(mapper, context, httpContextAccessor);
        }

        [TestMethod]
        public async Task RemoveCharacterShouldReturnFailWhenExceptionIsThrown()
        {
            //Arrange
            // var testId = 2;
            // var characters = new List<Character>
            // {
            //     new Character {Id = 1, Name = "character 1" },
            //     new Character {Id = testId, Name = "character 2" },
            //     new Character {Id = 3, Name = "character 3" },
            // };
            
            
            //Act
            // await characterService.Delete(testId);
            
            //Assert

        }
    }
}