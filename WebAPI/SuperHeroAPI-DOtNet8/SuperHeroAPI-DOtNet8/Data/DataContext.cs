using Microsoft.EntityFrameworkCore;
using SuperHeroAPI_DOtNet8.Entities;

namespace SuperHeroAPI_DOtNet8.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<SuperHero> SuperHeroes { get; set; }
    }
}
