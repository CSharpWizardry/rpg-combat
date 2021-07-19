using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            this.characterService = A.Fake<ICharacterService>();
            controller = new CharacterController(characterService);
        }

        [TestMethod]
        public async Task GetAllCharactersShouldReturnList()
        {
            //Arrange
            int count = 10;            
            var fakeData = A.CollectionOfFake<GetCharacterDto>(count).AsEnumerable();
            A.CallTo(() => characterService.GetAll()).Returns(Task.FromResult(fakeData));
            
            
            //Act
            var actionResult = await controller.Get();
            
            //Assert
            var result = actionResult.Result as OkObjectResult;
            var returnCharacters = result.Value as IEnumerable<GetCharacterDto>;
            Assert.AreEqual(count, returnCharacters.Count());
            A.CallTo(() => characterService.GetAll()).MustHaveHappened();
        }

        [TestMethod]
        public async Task GetSpecificCharacterShouldReturnItWhenItExists()
        {
            //Arrange
            int testId = 3;
            var character = new GetCharacterDto {Id = testId, Name = "character 3"};
            A.CallTo(() => characterService.GetById(A<int>.Ignored)).ReturnsLazily((int id) => id == testId ? Task.FromResult(character) : Task.FromResult<GetCharacterDto>(null));

            //Act
            var actionResult = await controller.GetSingle(testId);
            
            //Assert
            var result = actionResult.Result as OkObjectResult;
            var returnCharacter = result.Value as GetCharacterDto;
            Assert.AreEqual(testId, returnCharacter.Id);
            Assert.AreEqual("character 3", returnCharacter.Name);
            A.CallTo(() => characterService.GetById(testId)).MustHaveHappened();
        }

        [TestMethod]
        public async Task GetSpecificCharacterShouldReturnBadRequestWhenItDoesntExist()
        {
            //Arrange
            int testId = 5;
            A.CallTo(() => characterService.GetById(A<int>.Ignored)).Returns(Task.FromResult<GetCharacterDto>(null));

            //Act
            var actionResult = await controller.GetSingle(testId);
            
            //Assert
            var result = actionResult.Result as NotFoundObjectResult;            
            Assert.IsNull(result);
            A.CallTo(() => characterService.GetById(testId)).MustHaveHappened();
        }
    }
}
