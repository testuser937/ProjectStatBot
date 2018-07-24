using ModulBot.Models;
using Microsoft.EntityFrameworkCore;

namespace ModulBot.Database
{
    public class StatDbturnContext : DbContext
    {
        public DbSet<Statistic> Statistics { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Constants.ConnectionString);
        }
    }
}
