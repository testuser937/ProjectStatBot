using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatBot.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Extensions.Configuration;

namespace StatBot.Database
{
    public class BotDbContext: DbContext
    {
        
        //public BotDbContext() : base()
        //{
            
        //}
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PostgreSQL"));
            optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=1234;Database=postgres");
        }
    }
}
