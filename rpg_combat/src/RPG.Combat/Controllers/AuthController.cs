using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RPG.Combat.Data;
using RPG.Combat.Dtos.User;
using RPG.Combat.Models;

namespace RPG.Combat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            this.authRepository = authRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var user = new User 
            {
                Username = userRegisterDto.Username
            };
            var serviceResponse = await this.authRepository.Register(user, userRegisterDto.Password);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse);
            
            return CreatedAtAction(nameof(Register), new { Id = serviceResponse.Data}, serviceResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {            
            var serviceResponse = await this.authRepository.Login(request.Username, request.Password);

            if (!serviceResponse.Success)
                return BadRequest(serviceResponse);
            
            return Ok(serviceResponse);
        }
    }
}