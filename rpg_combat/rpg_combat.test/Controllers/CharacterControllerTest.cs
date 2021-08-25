using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using rpg_combat.Controllers;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;
using rpg_combat.Services.CharacterService;
using rpg_combat.Services.CharacterSkillService;
using rpg_combat.Services.LifeLogService;
using rpg_combat.Services.WeaponService;

namespace rpg_combat.test
{
    [TestClass]
    public class CharacterControllerTest
    {
        private CharacterController controller;
        private ICharacterService characterService;
        private ILifeLogService lifeLogService;

        [TestInitialize()]
        public void Initialize() 
        {
            this.characterService = Substitute.For<ICharacterService>();
            IWeaponService weaponService = Substitute.For<IWeaponService>();
            ICharacterSkillService characterSkillService = Substitute.For<ICharacterSkillService>();
            lifeLogService = Substitute.For<ILifeLogService>();

            controller = new CharacterController(characterService, weaponService, characterSkillService, lifeLogService, NullLogger<CharacterController>.Instance);
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

        [TestMethod]
        public async Task GetSpecificCharacterShouldReturnItWhenItExists()
        {
            //Arrange
            int testId = 3;
            var character = CreateGetCharacterDto(testId);
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

        [TestMethod]
        public async Task DeleteCharacterShouldReturnNoContentWhenSuccessfulExecuted()
        {
            //Arrange
            int id = 1;
            characterService.GetById(id).Returns(Task.FromResult(CreateGetCharacterDto(id)));

            //Act
            var actionResult = await controller.Delete(id);

            //Assert
            var result = actionResult as NoContentResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteCharacterShouldReturnNotFoundWhenCharacterDoesntExist()
        {
            //Arrange
            int id = 10;
            characterService.GetById(Arg.Any<int>()).Returns(Task.FromResult<GetCharacterDto>(null));

            //Act
            var actionResult = await controller.Delete(id);

            //Assert
            var result = actionResult as NotFoundResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task CreateCharacterShouldReturnNoContentWhenCharacterCreated()
        {
            //Arrange            
            var characterRequest = new AddCharacterDto
            {
                Defense = 10,
                HitPoints = 100,
                Intelligence = 10,
                Strength = 10
            };
            characterService.Add(characterRequest).Returns(Task.FromResult(CreateGetCharacterDto(1)));

            //Act
            var actionResult = await controller.Create(characterRequest);

            //Assert
            var result = actionResult as CreatedAtActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCharacterShouldReturnBadRequestWhenCharacterAndCharacterIdDoesntMatch()
        {
            //Arrange
            const int testId = 1;
            var characterRequest = new UpdateCharacterDto
            {
                Id = testId + 1,
                Defense = 10,
                HitPoints = 100
            };

            //Act
            var actionResult = await controller.Update(testId, characterRequest);

            //Assert
            Assert.IsNotNull(actionResult);
            var result = actionResult as BadRequestResult;
            Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCharacterShouldReturnNotFoundWhenCharacterDoesntExist()
        {
            //Arrange
            const int testId = 1;
            var characterRequest = new UpdateCharacterDto
            {
                Id = testId ,
                Defense = 10,
                HitPoints = 100
            };

            characterService.GetById(testId).Returns(Task.FromResult<GetCharacterDto>(null));

            //Act
            var actionResult = await controller.Update(testId, characterRequest);

            //Assert
            Assert.IsNotNull(actionResult);
            var result = actionResult as NotFoundResult;
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCharacterShouldReturnNoContentWhenCharacterIsSuccessfulyUpdated()
        {
            //Arrange
            const int testId = 1;
            var characterRequest = new UpdateCharacterDto
            {
                Id = testId,
                Defense = 10,
                HitPoints = 100
            };

            characterService.GetById(testId).Returns(Task.FromResult<GetCharacterDto>(CreateGetCharacterDto(testId)));
            characterService.Update(characterRequest).Returns(Task.FromResult(CreateGetCharacterDto(testId)));

            //Act
            var actionResult = await controller.Update(testId, characterRequest);

            //Assert
            Assert.IsNotNull(actionResult);
            var result = actionResult as NoContentResult;
            Assert.AreEqual(StatusCodes.Status204NoContent, result.StatusCode);

        }

        [TestMethod]
        public async Task GetLifeLogShouldReturnNotFoundWhenCharacterDoesntExistOrLifeLogForCharacterDoesntExist()
        {
            //Arrange
            const int testId = 1;
            var mockResponse = ServiceResponse<List<GetLifeLogDto>>.FailedFrom("Character not found");
            lifeLogService.FromCharacter(testId).Returns(Task.FromResult<ServiceResponse<List<GetLifeLogDto>>>(mockResponse));

            //Act
            var actionResult = await controller.GetLifeLog(testId);

            //Assert
            Assert.IsNotNull(actionResult);
            var result = actionResult as NotFoundObjectResult;
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task GetLifeLogShouldReturnListOfLogsForCharacter()
        {
            //Arrange
            const int testId = 1;
            var character = CreateCharacter(testId);
            List <GetLifeLogDto> logs = new List<GetLifeLogDto>
            { 
                LifeLog.CreateBornLog(character).ConvertToDto(),
                LifeLog.CreateVictoryLog(character, "sword", "Opponent").ConvertToDto(),
                LifeLog.CreateVictoryLog(character, "sword", "Opponent").ConvertToDto()
            };
            var mockResponse = ServiceResponse<List<GetLifeLogDto>>.From(logs);
            lifeLogService.FromCharacter(testId).Returns(Task.FromResult(mockResponse));

            //Act
            var actionResult = await controller.GetLifeLog(testId);

            //Assert
            Assert.IsNotNull(actionResult);
            var result = actionResult as OkObjectResult;
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        #region utility methods
        private static GetCharacterDto CreateGetCharacterDto(int id) => new GetCharacterDto { Id = id, Name = $"character {id}" };
        private static Character CreateCharacter(int id) => new Character {Id = id, Name = $"character {id}", Class = CharacterClass.Fighter, HitPoints = 100 };

        private IEnumerable<GetCharacterDto> GetFakeCharacterDtoList(int count = 10)
        {
            var list = new List<GetCharacterDto>();
            for (int i = 0; i < count; i++)
                list.Add(CreateGetCharacterDto(i));
            return list;
        }
        #endregion
    }
}
