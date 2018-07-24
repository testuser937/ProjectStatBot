using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModulBot.Models;

namespace ModulBot.Database.PostgresRepositories
{
    public class PostgresStatsRepository : IRepository<Statistic>
    {
        private readonly StatDbturnContext _turnContext;

        public PostgresStatsRepository()
        {
            _turnContext = new StatDbturnContext();
        }

        public void Add(Statistic obj)
        {
            var count = GetAll().ToList().Count;
            obj.Id = count + 1;

            _turnContext.Statistics.Add(obj);
        }

        public void Delete(int id)
        {
            var stat = _turnContext.Statistics.Find(id);
            if (stat != null)
            {
                _turnContext.Statistics.Remove(stat);
            }
        }

        public void Dispose()
        {
            _turnContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public IEnumerable<Statistic> GetAll()
        {
            return _turnContext.Statistics;
        }

        public Statistic GetById(int id)
        {
            return _turnContext.Statistics.Find(id);
        }

        public void Save()
        {
            _turnContext.SaveChanges();
        }

        public void Update(Statistic obj)
        {
            _turnContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
