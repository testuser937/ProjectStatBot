using StatBot.Models;
using Microsoft.EntityFrameworkCore;

namespace StatBot.Database
{
    public class StatDbContext : DbContext
    {
        public DbSet<Statistic> Statistics { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=1234;Database=postgres");
            
        }
    }
}
