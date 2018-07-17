using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatBot.Database
{
    interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T obj);

        void Update(T obj);

        void Delete(int id);

        void Save();
    }
}
