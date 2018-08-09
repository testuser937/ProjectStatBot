using System;
using System.Collections.Generic;

namespace ModulBot.Database
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
