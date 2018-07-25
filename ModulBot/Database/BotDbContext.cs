using Microsoft.EntityFrameworkCore;
using ModulBot.Models;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace ModulBot.Database
{
    public class BotDbturnContext : DbContext
    {
        public BotDbturnContext() { }

        private IConfiguration Configuration { get; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"));
        }
    }
}