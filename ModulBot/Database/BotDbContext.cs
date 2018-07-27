using Microsoft.EntityFrameworkCore;
using ModulBot.Models;

namespace ModulBot.Database
{
    public class BotDbturnContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Startup.GetConnectionString());
        }
    }
}