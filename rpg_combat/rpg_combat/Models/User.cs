using System.Collections.Generic;

namespace rpg_combat.Models
{
    public class User
    {
        public int Id { get;set;}
        public string Username { get; set; }  
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        List<Character> Characters { get; set; }
    }
}