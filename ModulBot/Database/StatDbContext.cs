using ModulBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ModulBot.Database
{
    public class StatDbturnContext : DbContext
    {
        private IConfiguration Configuration { get; }
        public DbSet<Statistic> Statistics { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"));
        }
    }
}
