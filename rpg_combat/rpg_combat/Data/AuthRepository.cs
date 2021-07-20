using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rpg_combat.Models;

namespace rpg_combat.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));
            if (user is null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return ServiceResponse<string>.FailedFrom("Invalid credentials or user doesn't exist");
            
            return ServiceResponse<string>.From(user.Id.ToString());
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            if (await UserExists(user.Username))
                return ServiceResponse<int>.FailedFrom($"username {user.Username} already taken.");
            
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return ServiceResponse<int>.From(user.Id);
        }

        public async Task<bool> UserExists(string username)
        {
            return (await context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()));
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {                
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(var i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }
    }
}