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
            SeedSkills(modelBuilder);
        }

        private static void SeedSkills(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
                new Skill {Id = 1, Damage = 15, Name = "Frenzy" },
                new Skill {Id = 2, Damage = 20, Name = "Blizzard" },
                new Skill {Id = 3, Damage = 11, Name = "Radiant Spear" }
            );
        }
    }
}