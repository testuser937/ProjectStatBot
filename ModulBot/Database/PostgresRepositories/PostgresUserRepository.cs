using System;
using System.Collections.Generic;
using System.Linq;
using ModulBot.Models;

namespace ModulBot.Database.PostgresRepositories
{
    public class PostgresUserRepository : IRepository<User>
    {
        private readonly BotDbturnContext _turnContext;
        public PostgresUserRepository()
        {
            _turnContext = new BotDbturnContext();
        }

        public void Add(User obj)
        {
            var count = GetAll().ToList().Count;
            obj.Id = count + 1;

            _turnContext.Users.Add(obj);
        }

        public void Delete(int id)
        {
            var user = _turnContext.Users.Find(id);
            if (user != null)
            {
                _turnContext.Users.Remove(user);
            }
        }

        public void Dispose()
        {
            _turnContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public IEnumerable<User> GetAll()
        {
            return _turnContext.Users;
        }

        public User GetById(int id)
        {
            return _turnContext.Users.Find(id);
        }

        public void Save()
        {
            _turnContext.SaveChanges();
        }

        public void Update(User obj)
        {
            _turnContext.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
