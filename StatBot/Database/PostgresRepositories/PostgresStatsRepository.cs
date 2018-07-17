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

        public void Add(Statistic obj)
        {
            var count = GetAll().ToList().Count;
            obj.Id = count + 1;

            _context.Statistics.Add(obj);
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null) _context.Dispose();
            }
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

        public void Update(Statistic obj)
        {
            _context.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
