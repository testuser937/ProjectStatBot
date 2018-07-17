using StatBot.Models;
using Microsoft.EntityFrameworkCore;

namespace StatBot.Database
{
    public class BotDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Constants.ConnectionString);
        }
    }
}