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
    }
}