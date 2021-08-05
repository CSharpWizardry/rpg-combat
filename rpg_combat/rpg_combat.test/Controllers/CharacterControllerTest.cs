using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using rpg_combat.Controllers;
using rpg_combat.Dtos.Character;
using rpg_combat.Services.CharacterService;

namespace rpg_combat.test
{
    [TestClass]
    public class CharacterControllerTest
    {
        private CharacterController controller;
        private ICharacterService characterService;

        [TestInitialize()]
        public void Initialize() 
        {
            this.characterService = Substitute.For<ICharacterService>();
            controller = new CharacterController(characterService);
        }

        [TestMethod]
        public async Task GetAllCharactersShouldReturnList()
        {
            //Arrange
            int count = 10;
            var fakeData = GetFakeCharacterDtoList(count);
            characterService.GetAll().Returns(Task.FromResult(fakeData));
            
            //Act
            var actionResult = await controller.Get();
            
            //Assert
            var result = actionResult.Result as OkObjectResult;
            var returnCharacters = result.Value as IEnumerable<GetCharacterDto>;
            Assert.AreEqual(count, returnCharacters.Count());
           await characterService.Received().GetAll();
        }

        private IEnumerable<GetCharacterDto> GetFakeCharacterDtoList(int count = 10)
        {
            var list = new List<GetCharacterDto>();
            for (int i = 0; i < count; i++)
                list.Add(Substitute.For<GetCharacterDto>());
            return list;
        }

        [TestMethod]
        public async Task GetSpecificCharacterShouldReturnItWhenItExists()
        {
            //Arrange
            int testId = 3;
            var character = new GetCharacterDto {Id = testId, Name = "character 3"};
            characterService.GetById(Arg.Any<int>()).Returns(Task.FromResult<GetCharacterDto>(null));
            characterService.GetById(testId).Returns(Task.FromResult(character));

            //Act
            var actionResult = await controller.GetSingle(testId);
            
            //Assert
            var result = actionResult.Result as OkObjectResult;
            var returnCharacter = result.Value as GetCharacterDto;
            Assert.AreEqual(testId, returnCharacter.Id);
            Assert.AreEqual("character 3", returnCharacter.Name);
            await characterService.Received().GetById(testId);
        }

        [TestMethod]
        public async Task GetSpecificCharacterShouldReturnNotFoundWhenItDoesntExist()
        {
            //Arrange
            int testId = 5;
            characterService.GetById(Arg.Any<int>()).Returns(Task.FromResult<GetCharacterDto>(null));

            //Act
            var actionResult = await controller.GetSingle(testId);
            
            //Assert
            var result = actionResult.Result as NotFoundObjectResult;            
            Assert.IsNull(result);
            await characterService.Received().GetById(testId);
        }
    }
}
