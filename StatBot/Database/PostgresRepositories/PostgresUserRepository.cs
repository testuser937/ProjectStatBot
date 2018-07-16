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

        public void Add(User user)
        {
            var count = GetAll().ToList().Count;
            user.Id = count + 1;

            _context.Users.Add(user);
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

        public void Update(User user)
        {
            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
    }
}
