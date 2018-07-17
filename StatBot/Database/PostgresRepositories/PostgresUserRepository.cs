using System;
using System.Collections.Generic;
using System.Linq;
using StatBot.Models;

namespace StatBot.Database.PostgresRepositories
{
    public class PostgresUserRepository : IRepository<User>
    {
        private readonly BotDbContext _context;
        public PostgresUserRepository()
        {
            _context = new BotDbContext();
        }

        public void Add(User obj)
        {
            var count = GetAll().ToList().Count;
            obj.Id = count + 1;

            _context.Users.Add(obj);
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
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

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(User obj)
        {
            _context.Entry(obj).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
