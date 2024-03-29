using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.Extensions;
using RPG.Combat.Data;
using RPG.Combat.Dtos.Character;
using RPG.Combat.Models;
using RPG.Combat.Services.CharacterService;

namespace RPG.Combat.Test.Services
{
    [TestClass]
    public class CharacterServiceTest
    {

        private static int userId = 1;
        private static int characterId = 1;
        private static IMapper mapper;
        private static IHttpContextAccessor httpContextAccessor;

        [TestInitialize()]
        public void Initialize() 
        {
            mapper = Substitute.For<IMapper>();
            httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            var identifierClaims = new Claim(ClaimTypes.NameIdentifier, userId.ToString());
            //var claims = new List<Claim>()
            //{
            //    new Claim(ClaimTypes.Name, "username"),
            //    ,
            //    new Claim("name", "John Doe"),
            //};
            //var identity = new ClaimsIdentity(claims, "TestAuthType");
            //var claimsPrincipal = new ClaimsPrincipal(identity);
            httpContextAccessor.HttpContext.User.FindFirst(Arg.Any<string>()).Returns(identifierClaims);
        }


        [TestMethod]
        public async Task GetAllCharactersShouldReturnEmptyListWhenThereAreNoCharacters()
        {
            //Arrange
            var opt = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new DataContext(opt))
            {
                var service = new CharacterService(mapper, context, httpContextAccessor, NullLogger<CharacterService>.Instance);

                //Act
                var result = await service.GetAll();

                //Assert
                Assert.IsNotNull(result);
                Assert.IsTrue(result.GetType() == typeof(List<GetCharacterDto>));
                Assert.AreEqual(expected: 0, result.Count());
            }
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

        #region utility methods
        private static List<Character> GetListOfCharacters()
            => new List<Character>
            {
                TestUtils.CreateCharacterWithUser(characterId, userId),
                TestUtils.CreateCharacterWithUser(characterId, 2),
                TestUtils.CreateCharacterWithUser(characterId, userId),
                TestUtils.CreateCharacterWithUser(characterId, 3),
                TestUtils.CreateCharacterWithUser(characterId, userId),
            };
        #endregion
    }
}