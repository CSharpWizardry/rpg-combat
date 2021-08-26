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
using rpg_combat.Data;
using rpg_combat.Dtos.Character;
using rpg_combat.Models;
using rpg_combat.Services.CharacterService;

namespace rpg_combat.test.Services
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
                GetCharacter(userId),
                GetCharacter(2),
                GetCharacter(userId),
                GetCharacter(3),
                GetCharacter(userId),
            };

        private static Character GetCharacter(int userId)
            => new Character
            {
                Id = characterId++,
                Class = CharacterClass.Wizard,
                HitPoints = 100,
                User = new User
                {
                    Id = userId,
                    Username = $"User {userId}"
                }            
            };
        #endregion
    }
}