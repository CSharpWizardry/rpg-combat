using Microsoft.EntityFrameworkCore;
using rpg_combat.Models;

namespace rpg_combat.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Character> Characters {get;set;}
        public DbSet<User> Users { get; set; }
        public DbSet<Weapon> Weapons { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<CharacterSkill> CharacterSkills { get; set; }
        public DbSet<LifeLog> LifeLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //add composed key to the CharacterSkill table
            modelBuilder.Entity<CharacterSkill>().HasKey(cs => new { cs.CharacterId, cs.SkillId });
        }
    }
}