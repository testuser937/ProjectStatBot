using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatBot.Database.PostgresRepositories
{
    public class PostgresStatsRepository : IRepository<Statistic>
    {
        private readonly StatDbContext _context;

        public PostgresStatsRepository()
        {
            _context = new StatDbContext();
        }

        public void Add(Statistic stat)
        {
            var count = GetAll().ToList().Count;
            stat.Id = count + 1;

            _context.Statistics.Add(stat);
        }

        public void Delete(int id)
        {
            var stat = _context.Statistics.Find(id);
            if (stat != null)
            {
                _context.Statistics.Remove(stat);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Statistic> GetAll()
        {
            return _context.Statistics;
        }

        public Statistic GetById(int id)
        {
            return _context.Statistics.Find(id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Statistic stat)
        {
            _context.Entry(stat).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
